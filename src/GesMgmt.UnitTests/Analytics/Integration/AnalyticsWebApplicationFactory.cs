using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GesMgmt.UnitTests.Analytics.Integration;

internal sealed class AnalyticsWebApplicationFactory(
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
            "ConnectionStrings:AvalAnalyticsConnection",
            "Server=localhost;Database=aval_analytics;Integrated Security=True;TrustServerCertificate=True;");

        builder.ConfigureServices(services =>
        {
            configureTestServices?.Invoke(services);
        });
    }
}
