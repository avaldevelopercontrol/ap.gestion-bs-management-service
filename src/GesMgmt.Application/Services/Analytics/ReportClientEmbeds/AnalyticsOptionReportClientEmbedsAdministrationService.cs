using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Application.Validators.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;

namespace GesMgmt.Application.Services.Analytics;

public sealed class AnalyticsOptionReportClientEmbedsAdministrationService(
    IAnalyticsReportClientConfigurationService configurationService,
    IAnalyticsReportClientPublicationWriter publicationWriter,
    IAnalyticsPowerBiSecurityPolicy powerBiSecurityPolicy)
    : IAnalyticsOptionReportClientEmbedsAdministrationService
{
    public async Task<AnalyticsOptionReportClientEmbedsResponse> GetAsync(
        int optionId,
        CancellationToken cancellationToken)
    {
        var configurations = await configurationService.ResolveAsync(
            optionId,
            cancellationToken);
        var clients = configurations
            .Select(AnalyticsReportClientConfigurationContractMapper.Map)
            .ToArray();

        return new AnalyticsOptionReportClientEmbedsResponse(
            optionId,
            clients);
    }

    public async Task<AnalyticsAdministrationCommandResult> UpdateAsync(
        int optionId,
        IReadOnlyList<UpdateAnalyticsOptionReportClientEmbed>? publications,
        int adminUserId,
        CancellationToken cancellationToken)
    {
        var configurations = await configurationService.ResolveAsync(
            optionId,
            cancellationToken);
        var publicationValidation = AnalyticsReportClientPublicationUpdateValidator.Validate(
            publications,
            configurations,
            powerBiSecurityPolicy.AllowPublishToWeb);

        if (publicationValidation.Error is not null)
        {
            return AnalyticsAdministrationCommandResult.Invalid(
                publicationValidation.Error.Title,
                publicationValidation.Error.Detail);
        }

        await publicationWriter.PatchAsync(
            optionId,
            publicationValidation.Updates,
            adminUserId,
            cancellationToken);

        return AnalyticsAdministrationCommandResult.Success();
    }
}
