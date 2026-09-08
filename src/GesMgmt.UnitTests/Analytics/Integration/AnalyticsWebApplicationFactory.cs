using GesMgmt.Infraestructure.Persistence.Analytics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GesMgmt.UnitTests.Analytics.Integration;

internal sealed class AnalyticsWebApplicationFactory(
    bool healthyDependencies = false,
    Action<IServiceCollection>? configureTestServices = null)
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting(
            "ConnectionStrings:AvalCobConnection",
            "Server=localhost;Database=aval_cob;Integrated Security=True;TrustServerCertificate=True;");
        builder.UseSetting(
            "ConnectionStrings:Analytics",
            "Server=localhost;Database=aval_analytics;Integrated Security=True;TrustServerCertificate=True;");

        builder.ConfigureServices(services =>
        {
            if (healthyDependencies)
            {
                services.RemoveAll<IAnalyticsDatabaseHealthProbe>();
                services.RemoveAll<ISisgesDatabaseHealthProbe>();
                services.AddSingleton<IAnalyticsDatabaseHealthProbe>(
                    new HealthyProbe("Base Analytics disponible."));
                services.AddSingleton<ISisgesDatabaseHealthProbe>(
                    new HealthySisgesProbe("Base SISGES disponible."));
            }

            configureTestServices?.Invoke(services);
        });
    }

    private sealed class HealthyProbe(string description) : IAnalyticsDatabaseHealthProbe
    {
        public Task<DatabaseHealthProbeResult> CheckAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new DatabaseHealthProbeResult(true, "none", description));
    }

    private sealed class HealthySisgesProbe(string description) : ISisgesDatabaseHealthProbe
    {
        public Task<DatabaseHealthProbeResult> CheckAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new DatabaseHealthProbeResult(true, "none", description));
    }
}
