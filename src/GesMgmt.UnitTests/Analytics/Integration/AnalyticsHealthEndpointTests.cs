using System.Net;
using System.Text.Json;

namespace GesMgmt.UnitTests.Analytics.Integration;

public sealed class AnalyticsHealthEndpointTests
{
    [Fact]
    public async Task Live_IsAvailableWithoutDatabaseAccess()
    {
        await using var factory = new AnalyticsWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health/live");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.Contains("X-Trace-Id"));
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Ready_ReportsBothAnalyticsDependencies()
    {
        await using var factory = new AnalyticsWebApplicationFactory(healthyDependencies: true);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.Contains("X-Trace-Id"));

        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("Healthy", body.RootElement.GetProperty("status").GetString());

        var checks = body.RootElement.GetProperty("checks")
            .EnumerateArray()
            .Select(item => item.GetProperty("name").GetString())
            .ToArray();

        Assert.Contains("analytics_database", checks);
        Assert.Contains("sisges_database", checks);
    }
}
