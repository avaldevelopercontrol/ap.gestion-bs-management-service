using System.Diagnostics;

namespace GesMgmt.WebAPI.Services.Analytics;

internal static class AnalyticsTraceContext
{
    public const string ResponseHeaderName = "X-Trace-Id";

    public static string GetTraceId(HttpContext context)
    {
        var activity = Activity.Current;
        return activity is null
            ? context.TraceIdentifier
            : activity.TraceId.ToString();
    }

    public static void SetResponseHeader(HttpContext context) =>
        context.Response.Headers[ResponseHeaderName] = GetTraceId(context);
}
