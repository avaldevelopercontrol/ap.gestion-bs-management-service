using System.Reflection;
using System.Text.RegularExpressions;
using GesMgmt.WebAPI.Controllers.Analytics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace GesMgmt.UnitTests.Analytics.Architecture;

public sealed class AnalyticsRouteContractTests
{
    [Fact]
    public void Controllers_PreserveTheThirtyTwoPublicAnalyticsOperations()
    {
        var actual = typeof(AnalyticsControllerBase).Assembly
            .GetTypes()
            .Where(type =>
                !type.IsAbstract &&
                typeof(AnalyticsControllerBase).IsAssignableFrom(type))
            .SelectMany(GetOperations)
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();

        var expected = new[]
        {
            "GET /api/v1/analytics-access/options",
            "GET /api/v1/analytics-access/options/{optionId}/clients",
            "GET /api/v1/analytics-access/options/{optionId}/groups",
            "GET /api/v1/analytics-access/options/{optionId}/power-bi-configuration",
            "GET /api/v1/analytics-access/options/{optionId}/report-client-embeds",
            "GET /api/v1/analytics-access/options/{optionId}/users",
            "GET /api/v1/analytics-access/user/options",
            "GET /api/v1/analytics-access/user/options/{optionId}/client-scopes",
            "GET /api/v1/analytics-access/user/options/{optionId}/clients",
            "GET /api/v1/analytics-access/user/options/{optionId}/group-scopes",
            "GET /api/v1/analytics-access/user/options/{optionId}/power-bi-viewer-context",
            "GET /api/v1/analytics-access/user/options/{optionId}/report-client-embed",
            "GET /api/v1/analytics-access/user/options/{optionId}/report-clients",
            "GET /api/v1/analytics-access/user/power-bi-access",
            "PATCH /api/v1/analytics-access/options/{optionId}/power-bi-configuration",
            "PUT /api/v1/analytics-access/options/{optionId}",
            "PUT /api/v1/analytics-access/options/{optionId}/clients",
            "PUT /api/v1/analytics-access/options/{optionId}/groups",
            "PUT /api/v1/analytics-access/options/{optionId}/report-client-embeds",
            "PUT /api/v1/analytics-access/options/{optionId}/users",
            "GET /api/v1/portfolio-control-center/advisor-performance",
            "GET /api/v1/portfolio-control-center/bootstrap",
            "GET /api/v1/portfolio-control-center/campaign-performance",
            "GET /api/v1/portfolio-control-center/evolution",
            "GET /api/v1/portfolio-control-center/filter-options",
            "GET /api/v1/portfolio-control-center/overview",
            "GET /api/v1/portfolio-control-center/promises",
            "GET /api/v1/portfolio-control-center/promises/due-today",
            "GET /api/v1/portfolio-control-center/promises/overdue",
            "GET /api/v1/portfolio-control-center/summary",
            "GET /api/v1/portfolio-control-center/supervisor-performance",
            "GET /api/v1/portfolio-control-center/target-progress"
        }.OrderBy(value => value, StringComparer.Ordinal).ToArray();

        Assert.Equal(expected, actual);
    }

    private static IEnumerable<string> GetOperations(Type controllerType)
    {
        var controllerRoute = controllerType
            .GetCustomAttributes<RouteAttribute>(inherit: true)
            .Single()
            .Template;

        foreach (var method in controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public))
        {
            foreach (var httpAttribute in method.GetCustomAttributes<HttpMethodAttribute>(inherit: true))
            {
                var route = Combine(controllerRoute, httpAttribute.Template);

                foreach (var httpMethod in httpAttribute.HttpMethods)
                {
                    yield return $"{httpMethod.ToUpperInvariant()} {route}";
                }
            }
        }
    }

    private static string Combine(string? controllerRoute, string? actionRoute)
    {
        var combined = string.Join(
            '/',
            new[] { controllerRoute, actionRoute }
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value!.Trim('/')));

        combined = Regex.Replace(combined, @":int(?=})", string.Empty);
        return "/" + combined;
    }
}
