using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Application.Validators.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;

namespace GesMgmt.Application.Services.Analytics;

public sealed class AnalyticsPowerBiViewerContextService(
    IAnalyticsOptionAccessService optionAccessService,
    IAnalyticsReportClientConfigurationService configurationService)
    : IAnalyticsPowerBiViewerContextService
{
    public async Task<AnalyticsPowerBiViewerContext> ResolveAsync(
        int userId,
        int optionId,
        AnalyticsPowerBiViewerSelection? selection,
        CancellationToken cancellationToken)
    {
        if (userId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(userId));
        }

        if (optionId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(optionId));
        }

        var optionAccess = await optionAccessService.ResolveAsync(
            userId,
            optionId,
            cancellationToken);

        if (!optionAccess.Allowed)
        {
            return Denied();
        }

        var requiresClientSelection =
            await configurationService.RequiresClientSelectionAsync(
                optionId,
                cancellationToken);

        if (!requiresClientSelection)
        {
            return new AnalyticsPowerBiViewerContext(
                Allowed: true,
                RequiresClientSelection: false,
                ClientSelectionStatus: AnalyticsPowerBiClientSelectionStatus.NotRequired,
                SelectedClient: null,
                EmbedUrl: null);
        }

        if (selection is null)
        {
            return new AnalyticsPowerBiViewerContext(
                Allowed: true,
                RequiresClientSelection: true,
                ClientSelectionStatus: AnalyticsPowerBiClientSelectionStatus.Missing,
                SelectedClient: null,
                EmbedUrl: null);
        }

        var activeUserGroupIds = optionAccess.ActiveUserGroupIds.ToHashSet();
        var configurations = await configurationService.ResolveAsync(
            optionId,
            cancellationToken);

        var authorizedConfiguration = configurations.FirstOrDefault(configuration =>
            AnalyticsReportClientAuthorization.Matches(
                configuration,
                selection.ClientId,
                selection.Name) &&
            AnalyticsReportClientAuthorization.IsAuthorized(
                configuration,
                activeUserGroupIds));

        if (authorizedConfiguration is null)
        {
            return new AnalyticsPowerBiViewerContext(
                Allowed: true,
                RequiresClientSelection: true,
                ClientSelectionStatus: AnalyticsPowerBiClientSelectionStatus.Invalid,
                SelectedClient: null,
                EmbedUrl: null);
        }

        if (!PowerBiPublishToWebUrl.TryNormalize(
            authorizedConfiguration.EmbedUrl,
            out var embedUrl))
        {
            return new AnalyticsPowerBiViewerContext(
                Allowed: true,
                RequiresClientSelection: true,
                ClientSelectionStatus: AnalyticsPowerBiClientSelectionStatus.Invalid,
                SelectedClient: null,
                EmbedUrl: null);
        }

        return new AnalyticsPowerBiViewerContext(
            Allowed: true,
            RequiresClientSelection: true,
            ClientSelectionStatus: AnalyticsPowerBiClientSelectionStatus.Valid,
            SelectedClient: new AnalyticsReportClientOption(
                authorizedConfiguration.ClientId,
                authorizedConfiguration.Name),
            EmbedUrl: embedUrl);
    }

    private static AnalyticsPowerBiViewerContext Denied() =>
        new(
            Allowed: false,
            RequiresClientSelection: false,
            ClientSelectionStatus: AnalyticsPowerBiClientSelectionStatus.NotRequired,
            SelectedClient: null,
            EmbedUrl: null);
}
