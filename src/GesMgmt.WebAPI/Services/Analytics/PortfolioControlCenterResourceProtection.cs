using System.Threading.RateLimiting;
using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GesMgmt.WebAPI.Services.Analytics;

internal static class PortfolioControlCenterResourceProtection
{
    public const string ConcurrencyPolicyName =
        "portfolio-control-center-concurrency";

    public const string RequestTimeoutPolicyName =
        "portfolio-control-center-timeout";

    public static IServiceCollection AddPortfolioControlCenterResourceProtection(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var performance = configuration
            .GetSection(PortfolioControlCenterPerformanceOptions.SectionName)
            .Get<PortfolioControlCenterPerformanceOptions>()
            ?? new PortfolioControlCenterPerformanceOptions();

        performance.Validate();

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status503ServiceUnavailable;
            options.OnRejected = static async (context, _) =>
            {
                await WriteProblemAsync(
                    context.HttpContext,
                    StatusCodes.Status503ServiceUnavailable,
                    "Capacidad temporalmente agotada",
                    "Portfolio Control Center está procesando el máximo de solicitudes concurrentes permitido. Intente nuevamente en unos instantes.");
            };

            options.AddConcurrencyLimiter(
                ConcurrencyPolicyName,
                limiter =>
                {
                    limiter.PermitLimit = performance.MaxConcurrentRequests;
                    limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    limiter.QueueLimit = performance.QueueLimit;
                });
        });

        services.AddRequestTimeouts(options =>
        {
            options.AddPolicy(
                RequestTimeoutPolicyName,
                new RequestTimeoutPolicy
                {
                    Timeout = TimeSpan.FromSeconds(
                        performance.RequestTimeoutSeconds),
                    TimeoutStatusCode = StatusCodes.Status503ServiceUnavailable,
                    WriteTimeoutResponse = static context => WriteProblemAsync(
                        context,
                        StatusCodes.Status503ServiceUnavailable,
                        "Tiempo de procesamiento agotado",
                        "Portfolio Control Center excedió el tiempo máximo de procesamiento permitido. La operación fue cancelada para proteger los recursos del servicio.")
                });
        });

        return services;
    }

    private static async Task WriteProblemAsync(
        HttpContext context,
        int statusCode,
        string title,
        string detail)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.Clear();
        AnalyticsTraceContext.SetResponseHeader(context);

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
}
