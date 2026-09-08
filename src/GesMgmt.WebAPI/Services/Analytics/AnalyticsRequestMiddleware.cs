using System.Data.Common;
using System.Diagnostics;
using GesMgmt.Application.Utils.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence.Analytics;

namespace GesMgmt.WebAPI.Services.Analytics;

internal sealed class AnalyticsRequestMiddleware(
    RequestDelegate next,
    ILogger<AnalyticsRequestMiddleware> logger)
{
    private static readonly EventId DatabaseFailureEvent =
        new(1001, "AnalyticsDatabaseRequestFailure");

    private static readonly EventId UnhandledFailureEvent =
        new(1000, "AnalyticsUnhandledRequestFailure");

    public async Task InvokeAsync(HttpContext context)
    {
        var isAnalyticsAccess = context.Request.Path.StartsWithSegments(
            "/api/v1/analytics-access");
        var isPortfolioControlCenter = context.Request.Path.StartsWithSegments(
            "/api/v1/portfolio-control-center");

        if (!isAnalyticsAccess && !isPortfolioControlCenter)
        {
            await next(context);
            return;
        }

        AnalyticsTraceContext.SetResponseHeader(context);

        if (isAnalyticsAccess)
        {
            context.Response.OnStarting(() =>
            {
                context.Response.Headers["Cache-Control"] = "no-store, max-age=0";
                context.Response.Headers["Pragma"] = "no-cache";
                AnalyticsTraceContext.SetResponseHeader(context);
                return Task.CompletedTask;
            });
        }
        else
        {
            context.Response.OnStarting(() =>
            {
                AnalyticsTraceContext.SetResponseHeader(context);
                return Task.CompletedTask;
            });
        }

        var startedAt = Stopwatch.GetTimestamp();
        using var requestScope = isPortfolioControlCenter
            ? PortfolioControlCenterDiagnostics.BeginRequest(
                context.Request.Path.Value ?? "/api/v1/portfolio-control-center/unknown")
            : null;

        try
        {
            await next(context);

            if (!context.Response.HasStarted &&
                context.Response.StatusCode == StatusCodes.Status404NotFound &&
                string.IsNullOrWhiteSpace(context.Response.ContentType))
            {
                await WriteStatusCodeProblemAsync(
                    context,
                    StatusCodes.Status404NotFound,
                    "Recurso no encontrado",
                    "La ruta o el recurso Analytics solicitado no existe.",
                    isAnalyticsAccess);
            }
        }
        catch (OperationCanceledException)
            when (context.RequestAborted.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            if (context.Response.HasStarted)
            {
                throw;
            }

            await WriteFailureAsync(context, exception, isAnalyticsAccess);
        }
        finally
        {
            if (isPortfolioControlCenter)
            {
                LogPortfolioBaseline(
                    context,
                    Stopwatch.GetElapsedTime(startedAt),
                    PortfolioControlCenterDiagnostics.Snapshot());
            }
        }
    }

    private void LogPortfolioBaseline(
        HttpContext context,
        TimeSpan elapsed,
        PortfolioControlCenterRequestSnapshot snapshot)
    {
        logger.LogInformation(
            "Portfolio baseline endpoint={Endpoint} status={StatusCode} elapsed_ms={ElapsedMilliseconds:F2} access_ms={AccessMilliseconds:F2} context_ms={ContextMilliseconds:F2} query_ms={QueryMilliseconds:F2} db_commands={DatabaseCommandCount} db_connection_opens={ConnectionOpenCount} db_command_ms={DatabaseCommandMilliseconds:F2} db_analytics_commands={AnalyticsDatabaseCommands} db_sisges_commands={SisgesDatabaseCommands} db_access_commands={AccessDatabaseCommands} db_context_commands={ContextDatabaseCommands} db_query_commands={QueryDatabaseCommands} db_unclassified_commands={UnclassifiedDatabaseCommands}",
            snapshot.Endpoint,
            context.Response.StatusCode,
            elapsed.TotalMilliseconds,
            snapshot.AccessMilliseconds,
            snapshot.ContextMilliseconds,
            snapshot.QueryMilliseconds,
            snapshot.DatabaseCommandCount,
            snapshot.ConnectionOpenCount,
            snapshot.DatabaseCommandMilliseconds,
            snapshot.AnalyticsDatabaseCommands,
            snapshot.SisgesDatabaseCommands,
            snapshot.AccessDatabaseCommands,
            snapshot.ContextDatabaseCommands,
            snapshot.QueryDatabaseCommands,
            snapshot.UnclassifiedDatabaseCommands);
    }

    private static async Task WriteStatusCodeProblemAsync(
        HttpContext context,
        int statusCode,
        string title,
        string detail,
        bool disableCache)
    {
        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        AnalyticsTraceContext.SetResponseHeader(context);

        if (disableCache)
        {
            context.Response.Headers["Cache-Control"] = "no-store, max-age=0";
            context.Response.Headers["Pragma"] = "no-cache";
        }

        var problem = AnalyticsProblemDetailsFactory.Create(
            context,
            statusCode,
            title,
            detail);

        await Results.Json(
                problem,
                statusCode: statusCode,
                contentType: "application/problem+json")
            .ExecuteAsync(context);
    }

    private async Task WriteFailureAsync(
        HttpContext context,
        Exception exception,
        bool disableCache)
    {
        var category = AnalyticsDatabaseFailureClassifier.Classify(exception);
        var isDatabaseFailure = exception is DbException ||
            category != AnalyticsDatabaseFailureCategory.Unknown;
        var traceId = AnalyticsTraceContext.GetTraceId(context);

        context.Response.Clear();
        AnalyticsTraceContext.SetResponseHeader(context);

        if (disableCache)
        {
            context.Response.Headers["Cache-Control"] = "no-store, max-age=0";
            context.Response.Headers["Pragma"] = "no-cache";
        }

        if (isDatabaseFailure)
        {
            logger.LogError(
                DatabaseFailureEvent,
                exception,
                "Analytics database request failed. Method={Method} Path={Path} FailureCategory={FailureCategory} TraceId={TraceId}",
                context.Request.Method,
                context.Request.Path.Value,
                category.ToWireValue(),
                traceId);

            var problem = AnalyticsProblemDetailsFactory.Create(
                context,
                StatusCodes.Status503ServiceUnavailable,
                "Dependencia de datos temporalmente no disponible",
                "La API no pudo completar una operación contra una dependencia de datos. Revise /health/ready y use el traceId para correlacionar el incidente.",
                new Dictionary<string, object?>
                {
                    ["failureCategory"] = category.ToWireValue()
                });

            await Results.Json(
                    problem,
                    statusCode: StatusCodes.Status503ServiceUnavailable,
                    contentType: "application/problem+json")
                .ExecuteAsync(context);
            return;
        }

        logger.LogError(
            UnhandledFailureEvent,
            exception,
            "Unhandled Analytics request failure. Method={Method} Path={Path} TraceId={TraceId}",
            context.Request.Method,
            context.Request.Path.Value,
            traceId);

        var internalProblem = AnalyticsProblemDetailsFactory.Create(
            context,
            StatusCodes.Status500InternalServerError,
            "Error interno del servidor",
            "Ocurrió un error interno inesperado. Use el traceId para correlacionar el incidente con los logs del servidor.");

        await Results.Json(
                internalProblem,
                statusCode: StatusCodes.Status500InternalServerError,
                contentType: "application/problem+json")
            .ExecuteAsync(context);
    }
}
