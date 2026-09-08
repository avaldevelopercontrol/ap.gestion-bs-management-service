using System.Security.Claims;
using GesMgmt.WebAPI.Services.Analytics;
using Microsoft.AspNetCore.Http;

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
        var context = new HttpAnalyticsUserContext(accessor);

        var result = context.TryGetUserId(out var userId);

        Assert.True(result);
        Assert.Equal(16068, userId);
    }

    [Fact]
    public void TryGetUserId_IgnoresDevelopmentHeader()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["X-Sisges-User-Id"] = "16068";
        var accessor = new HttpContextAccessor { HttpContext = httpContext };
        var context = new HttpAnalyticsUserContext(accessor);

        var result = context.TryGetUserId(out var userId);

        Assert.False(result);
        Assert.Equal(0, userId);
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
        var context = new HttpAnalyticsUserContext(accessor);

        Assert.False(context.TryGetUserId(out var userId));
        Assert.Equal(0, userId);
    }
}
