using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GesMgmt.WebAPI.Services.Analytics.Health
{
    internal static class AnalyticsHealthResponseWriter
    {
        private const string TraceHeaderName = "X-Trace-Id";

        private static readonly JsonSerializerOptions SerializerOptions =
            new(JsonSerializerDefaults.Web);

        public static Task WriteLiveAsync(
            HttpContext context,
            HealthReport report)
        {
            SetTraceHeader(context);
            context.Response.ContentType = "text/plain; charset=utf-8";
            return context.Response.WriteAsync(report.Status.ToString());
        }

        public static Task WriteReadyAsync(
            HttpContext context,
            HealthReport report)
        {
            SetTraceHeader(context);
            context.Response.ContentType = "application/json; charset=utf-8";

            var response = new AnalyticsHealthResponse(
                report.Status.ToString(),
                report.Entries
                    .OrderBy(entry => entry.Key, StringComparer.Ordinal)
                    .Select(entry => new AnalyticsHealthCheckResponse(
                        entry.Key,
                        entry.Value.Status.ToString(),
                        GetStringData(entry.Value, "failureCategory") ?? "none",
                        entry.Value.Description))
                    .ToArray());

            return context.Response.WriteAsync(
                JsonSerializer.Serialize(response, SerializerOptions));
        }

        private static void SetTraceHeader(HttpContext context)
        {
            var activity = Activity.Current;
            var traceId = activity is null
                ? context.TraceIdentifier
                : activity.TraceId.ToString();

            context.Response.Headers[TraceHeaderName] = traceId;
        }

        private static string? GetStringData(
            HealthReportEntry entry,
            string key)
        {
            if (!entry.Data.TryGetValue(key, out var value))
            {
                return null;
            }

            return Convert.ToString(value);
        }
    }

    internal sealed record AnalyticsHealthResponse(
        string Status,
        IReadOnlyList<AnalyticsHealthCheckResponse> Checks);

    internal sealed record AnalyticsHealthCheckResponse(
        string Name,
        string Status,
        string FailureCategory,
        string? Description);
}
