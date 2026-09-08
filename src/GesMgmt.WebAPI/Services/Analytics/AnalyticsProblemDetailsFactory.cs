using Microsoft.AspNetCore.Mvc;

namespace GesMgmt.WebAPI.Services.Analytics;

internal static class AnalyticsProblemDetailsFactory
{
    public static ProblemDetails Create(
        HttpContext context,
        int statusCode,
        string title,
        string detail,
        IReadOnlyDictionary<string, object?>? extensions = null)
    {
        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path.Value
        };

        problem.Extensions["traceId"] = AnalyticsTraceContext.GetTraceId(context);

        if (extensions is not null)
        {
            foreach (var extension in extensions)
            {
                problem.Extensions[extension.Key] = extension.Value;
            }
        }

        return problem;
    }
}
