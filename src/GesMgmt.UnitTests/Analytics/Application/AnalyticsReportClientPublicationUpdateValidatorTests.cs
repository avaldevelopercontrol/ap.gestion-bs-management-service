using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.Validators.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.UnitTests.Analytics.Application;

public sealed class AnalyticsReportClientPublicationUpdateValidatorTests
{
    [Fact]
    public void Validate_WhenOnlyUrlChanges_PreservesNullGroupIds()
    {
        var configuration = CreateConfiguration(
            AnalyticsReportClientGroupResolution.AutoDetected,
            false,
            [219]);

        var result = AnalyticsReportClientPublicationUpdateValidator.Validate(
            [new UpdateAnalyticsOptionReportClientEmbed(
                178,
                "ADEX INSTITUTO",
                null,
                "https://app.powerbi.com/view?r=test")],
            [configuration]);

        Assert.Null(result.Error);
        var update = Assert.Single(result.Updates);
        Assert.Null(update.GroupIds);
        Assert.Equal("https://app.powerbi.com/view?r=test", update.EmbedUrl);
    }

    [Fact]
    public void Validate_WhenRequestedGroupIsOutsideClient_ReturnsValidationError()
    {
        var configuration = CreateConfiguration(
            AnalyticsReportClientGroupResolution.Configured,
            true,
            [219]);

        var result = AnalyticsReportClientPublicationUpdateValidator.Validate(
            [new UpdateAnalyticsOptionReportClientEmbed(
                178,
                "ADEX INSTITUTO",
                [999],
                "https://app.powerbi.com/view?r=test")],
            [configuration]);

        Assert.NotNull(result.Error);
        Assert.Equal("Grupo fuera del cliente", result.Error?.Title);
        Assert.Empty(result.Updates);
    }

    [Fact]
    public void Validate_WhenPublishToWebIsDisabled_RejectsPublicUrl()
    {
        var configuration = CreateConfiguration(
            AnalyticsReportClientGroupResolution.Configured,
            true,
            [219]);

        var result = AnalyticsReportClientPublicationUpdateValidator.Validate(
            [new UpdateAnalyticsOptionReportClientEmbed(
                178,
                "ADEX INSTITUTO",
                [219],
                "https://app.powerbi.com/view?r=test")],
            [configuration],
            allowPublishToWeb: false);

        Assert.NotNull(result.Error);
        Assert.Equal("Publish to web deshabilitado", result.Error?.Title);
        Assert.Empty(result.Updates);
    }

    private static AnalyticsReportClientConfiguration CreateConfiguration(
        string groupResolution,
        bool hasExplicitGroupConfiguration,
        IReadOnlyList<int> groupIds) =>
        new(
            178,
            "ADEX INSTITUTO",
            true,
            groupResolution,
            hasExplicitGroupConfiguration,
            groupIds,
            [new AnalyticsReportClientConfigurationGroup(219, "ADEX INSTITUTO [219]")],
            "https://app.powerbi.com/view?r=old",
            true);
}
