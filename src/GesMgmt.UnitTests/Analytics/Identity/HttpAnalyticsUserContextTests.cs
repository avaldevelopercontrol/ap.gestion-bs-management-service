using System.Security.Claims;
using GesMgmt.WebAPI;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace GesMgmt.UnitTests.Analytics.Identity;

public sealed class HttpAnalyticsUserContextTests
{
    [Theory]
    [InlineData("sisges_user_id")]
    [InlineData("user_id")]
    [InlineData(ClaimTypes.NameIdentifier)]
    public void TryGetUserId_ReadsSupportedClaimFromAuthenticatedIdentity(string claimType)
    {
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    [new Claim(claimType, "16068")],
                    authenticationType: "host-test"))
        };
        var accessor = new HttpContextAccessor { HttpContext = httpContext };
        var context = CreateContext(accessor, Environments.Production);

        var result = context.TryGetUserId(out var userId);

        Assert.True(result);
        Assert.Equal(16068, userId);
    }

    [Fact]
    public void TryGetUserId_ReadsSisgesHeader()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["X-Sisges-User-Id"] = "16068";
        var accessor = new HttpContextAccessor { HttpContext = httpContext };
        var context = CreateContext(accessor, Environments.Development);

        var result = context.TryGetUserId(out var userId);

        Assert.True(result);
        Assert.Equal(16068, userId);
    }

    [Fact]
    public void TryGetUserId_ReadsSisgesHeaderOutsideDevelopment()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["X-Sisges-User-Id"] = "16068";
        var accessor = new HttpContextAccessor { HttpContext = httpContext };
        var context = CreateContext(accessor, Environments.Production);

        var result = context.TryGetUserId(out var userId);

        Assert.True(result);
        Assert.Equal(16068, userId);
    }

    [Theory]
    [InlineData("sisges_group_id")]
    [InlineData("group_id")]
    public void TryGetGroupId_ReadsSupportedClaimFromAuthenticatedIdentity(string claimType)
    {
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    [new Claim(claimType, "156")],
                    authenticationType: "host-test"))
        };
        var accessor = new HttpContextAccessor { HttpContext = httpContext };
        var context = CreateContext(accessor, Environments.Production);

        var result = context.TryGetGroupId(out var groupId);

        Assert.True(result);
        Assert.Equal(156, groupId);
    }

    [Fact]
    public void TryGetGroupId_ReadsSisgesHeader()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["X-Sisges-Group-Id"] = "156";
        var accessor = new HttpContextAccessor { HttpContext = httpContext };
        var context = CreateContext(accessor, Environments.Development);

        var result = context.TryGetGroupId(out var groupId);

        Assert.True(result);
        Assert.Equal(156, groupId);
    }

    [Fact]
    public void TryGetGroupId_ReadsSisgesHeaderOutsideDevelopment()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["X-Sisges-Group-Id"] = "156";
        var accessor = new HttpContextAccessor { HttpContext = httpContext };
        var context = CreateContext(accessor, Environments.Production);

        Assert.True(context.TryGetGroupId(out var groupId));
        Assert.Equal(156, groupId);
    }

    [Fact]
    public void TryGetUserId_IgnoresClaimsFromUnauthenticatedIdentity()
    {
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(
                new ClaimsIdentity([new Claim("sisges_user_id", "16068")]))
        };
        var accessor = new HttpContextAccessor { HttpContext = httpContext };
        var context = CreateContext(accessor, Environments.Production);

        Assert.False(context.TryGetUserId(out var userId));
        Assert.Equal(0, userId);
    }

    [Fact]
    public void TryGetUserId_UsesConfiguredTestingUserOnlyInDevelopment()
    {
        var accessor = new HttpContextAccessor { HttpContext = new DefaultHttpContext() };
        var context = CreateContext(accessor, Environments.Development, localTestingUserId: 16068);

        Assert.True(context.TryGetUserId(out var userId));
        Assert.Equal(16068, userId);
    }

    [Fact]
    public void TryGetUserId_DoesNotUseConfiguredTestingUserOutsideDevelopment()
    {
        var accessor = new HttpContextAccessor { HttpContext = new DefaultHttpContext() };
        var context = CreateContext(accessor, Environments.Production, localTestingUserId: 16068);

        Assert.False(context.TryGetUserId(out var userId));
        Assert.Equal(0, userId);
    }

    private static HttpAnalyticsUserContext CreateContext(
        IHttpContextAccessor accessor,
        string environmentName,
        int? localTestingUserId = null,
        int? localTestingGroupId = null)
    {
        var settings = new Dictionary<string, string?>();
        if (localTestingUserId is > 0)
        {
            settings["AnalyticsTesting:UserId"] = localTestingUserId.Value.ToString();
        }

        if (localTestingGroupId is > 0)
        {
            settings["AnalyticsTesting:GroupId"] = localTestingGroupId.Value.ToString();
        }

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        return new HttpAnalyticsUserContext(
            accessor,
            configuration,
            new TestHostEnvironment(environmentName));
    }

    private sealed class TestHostEnvironment(string environmentName) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = environmentName;
        public string ApplicationName { get; set; } = "GesMgmt.UnitTests";
        public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
