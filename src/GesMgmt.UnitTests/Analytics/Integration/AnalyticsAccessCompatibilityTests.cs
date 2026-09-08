using System.Data.Common;
using System.Net;
using System.Text.Json;
using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.Interfaces.Analytics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GesMgmt.UnitTests.Analytics.Integration;

public sealed class AnalyticsAccessCompatibilityTests
{
    [Fact]
    public async Task UserEndpoint_WithoutHostIdentity_ReturnsUnauthorizedProblemDetails()
    {
        await using var factory = new AnalyticsWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/analytics-access/user/options");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.True(response.Headers.TryGetValues("X-Trace-Id", out var traceValues));
        Assert.True(response.Headers.CacheControl?.NoStore == true);
        Assert.True(response.Headers.CacheControl?.MaxAge == TimeSpan.Zero);
        Assert.True(response.Headers.Pragma.Any(value =>
            string.Equals(value.Name, "no-cache", StringComparison.OrdinalIgnoreCase)));

        var traceHeader = Assert.Single(traceValues!);
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(traceHeader, body.RootElement.GetProperty("traceId").GetString());
    }

    [Fact]
    public async Task DevelopmentHeader_DoesNotAuthenticateConsolidatedHost()
    {
        await using var factory = new AnalyticsWebApplicationFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Sisges-User-Id", "16068");

        var response = await client.GetAsync("/api/v1/analytics-access/user/options");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UnknownAnalyticsRoute_ReturnsNotFoundProblemDetailsWithTraceId()
    {
        await using var factory = new AnalyticsWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/analytics-access/route-that-does-not-exist");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.True(response.Headers.Contains("X-Trace-Id"));
    }

    [Fact]
    public async Task DatabaseFailure_ReturnsServiceUnavailableWithoutLeakingException()
    {
        const string sensitiveMessage = "sensitive database failure";
        await using var factory = CreateThrowingFactory(new TestDbException(sensitiveMessage));
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/analytics-access/user/options");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.False(body.Contains(sensitiveMessage, StringComparison.Ordinal));
        Assert.True(body.Contains("failureCategory", StringComparison.Ordinal));
    }

    [Fact]
    public async Task UnexpectedFailure_ReturnsInternalServerErrorWithoutLeakingException()
    {
        const string sensitiveMessage = "sensitive unexpected failure";
        await using var factory = CreateThrowingFactory(new InvalidOperationException(sensitiveMessage));
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/analytics-access/user/options");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.False(body.Contains(sensitiveMessage, StringComparison.Ordinal));
    }

    private static AnalyticsWebApplicationFactory CreateThrowingFactory(Exception exception) =>
        new(
            configureTestServices: services =>
            {
                services.RemoveAll<IAnalyticsUserContext>();
                services.RemoveAll<IAnalyticsUserOptionQueryService>();
                services.AddSingleton<IAnalyticsUserContext>(new AuthenticatedUserContext(16068));
                services.AddSingleton<IAnalyticsUserOptionQueryService>(new ThrowingUserOptionsService(exception));
            });

    private sealed class AuthenticatedUserContext(int userId) : IAnalyticsUserContext
    {
        public bool TryGetUserId(out int currentUserId)
        {
            currentUserId = userId;
            return true;
        }
    }

    private sealed class ThrowingUserOptionsService(Exception exception) : IAnalyticsUserOptionQueryService
    {
        public Task<IReadOnlyList<AnalyticsUserOptionResponse>> GetAsync(
            int userId,
            CancellationToken cancellationToken) =>
            Task.FromException<IReadOnlyList<AnalyticsUserOptionResponse>>(exception);
    }

    private sealed class TestDbException(string message) : DbException(message);
}
