using System.Net.Sockets;
using System.Security.Authentication;
using GesMgmt.Infraestructure.Persistence.Analytics;

namespace GesMgmt.UnitTests.Analytics.Infrastructure;

public sealed class AnalyticsDatabaseFailureClassifierTests
{
    [Fact]
    public void Classify_RecognizesMissingConnectionConfiguration()
    {
        var category = AnalyticsDatabaseFailureClassifier.Classify(
            new InvalidOperationException("No se configuró ConnectionStrings:AvalAnalyticsConnection."));

        Assert.Equal(AnalyticsDatabaseFailureCategory.Configuration, category);
        Assert.Equal("configuration", category.ToWireValue());
    }

    [Fact]
    public void Classify_RecognizesTlsFailureInInnerChain()
    {
        var exception = new InvalidOperationException(
            "SQL connection failed.",
            new AuthenticationException("Authentication failed."));

        Assert.Equal(
            AnalyticsDatabaseFailureCategory.TlsHandshake,
            AnalyticsDatabaseFailureClassifier.Classify(exception));
    }

    [Fact]
    public void Classify_RecognizesConnectivityFailure()
    {
        var exception = new SocketException((int)SocketError.ConnectionRefused);

        Assert.Equal(
            AnalyticsDatabaseFailureCategory.Connectivity,
            AnalyticsDatabaseFailureClassifier.Classify(exception));
    }

    [Fact]
    public void Classify_FallsBackToUnknown()
    {
        Assert.Equal(
            AnalyticsDatabaseFailureCategory.Unknown,
            AnalyticsDatabaseFailureClassifier.Classify(
                new Exception("Unexpected database dependency failure.")));
    }
}
