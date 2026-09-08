using GesMgmt.Infraestructure.Persistence.Analytics;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GesMgmt.WebAPI.Services.Analytics.Health
{
    internal sealed class AnalyticsDatabaseHealthCheck(
        IAnalyticsDatabaseHealthProbe probe) : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var result = await probe.CheckAsync(cancellationToken);
            var data = new Dictionary<string, object>(StringComparer.Ordinal)
            {
                ["failureCategory"] = result.FailureCategory,
                ["dependency"] = "analytics_database"
            };

            return result.IsHealthy
                ? HealthCheckResult.Healthy(result.Description, data)
                : HealthCheckResult.Unhealthy(
                    result.Description,
                    data: data);
        }
    }
}
