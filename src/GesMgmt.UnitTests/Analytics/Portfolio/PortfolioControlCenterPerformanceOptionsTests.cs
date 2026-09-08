using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

namespace GesMgmt.UnitTests.Analytics.Portfolio;

public sealed class PortfolioControlCenterPerformanceOptionsTests
{
    [Fact]
    public void Defaults_PreserveOriginalResourceProtectionValues()
    {
        var options = new PortfolioControlCenterPerformanceOptions();

        Assert.Equal(80, options.MaxConcurrentRequests);
        Assert.Equal(240, options.QueueLimit);
        Assert.Equal(60, options.RequestTimeoutSeconds);
        Assert.Equal(30, options.DetailCacheSeconds);
        Assert.Equal(512, options.DetailCacheMaxEntries);
        options.Validate();
    }

    [Theory]
    [InlineData(0, 240, 60, 30, 512)]
    [InlineData(513, 240, 60, 30, 512)]
    [InlineData(80, -1, 60, 30, 512)]
    [InlineData(80, 4097, 60, 30, 512)]
    [InlineData(80, 240, 0, 30, 512)]
    [InlineData(80, 240, 301, 30, 512)]
    [InlineData(80, 240, 60, -1, 512)]
    [InlineData(80, 240, 60, 301, 512)]
    [InlineData(80, 240, 60, 30, 0)]
    [InlineData(80, 240, 60, 30, 4097)]
    public void Validate_InvalidValue_FailsFast(
        int maxConcurrentRequests,
        int queueLimit,
        int requestTimeoutSeconds,
        int detailCacheSeconds,
        int detailCacheMaxEntries)
    {
        var options = new PortfolioControlCenterPerformanceOptions
        {
            MaxConcurrentRequests = maxConcurrentRequests,
            QueueLimit = queueLimit,
            RequestTimeoutSeconds = requestTimeoutSeconds,
            DetailCacheSeconds = detailCacheSeconds,
            DetailCacheMaxEntries = detailCacheMaxEntries
        };

        Assert.Throws<InvalidOperationException>(options.Validate);
    }
}
