using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Application.Validators.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;

namespace GesMgmt.Application.Services.Analytics;

public sealed class AnalyticsPowerBiConfigurationService(
    IAnalyticsOptionConfigRepository optionRepository,
    IAnalyticsOptionGroupScopeRepository optionGroupRepository,
    ISisgesClientGroupRepository clientGroupRepository,
    IAnalyticsReportClientConfigurationService configurationService,
    IAnalyticsPowerBiSecurityPolicy powerBiSecurityPolicy,
    IAnalyticsPowerBiConfigurationWriter writer)
    : IAnalyticsPowerBiConfigurationService
{
    public async Task<AnalyticsPowerBiConfigurationResponse> GetAsync(
        int optionId,
        CancellationToken cancellationToken)
    {
        var existsTask = optionRepository.ExistsAsync(
            optionId,
            cancellationToken);
        var availableGroupsTask = clientGroupRepository.GetAllActiveGroupsAsync(
            cancellationToken);

        await Task.WhenAll(existsTask, availableGroupsTask);

        var availableGroups = (await availableGroupsTask)
            .Where(group => group.GroupId > 0 && group.ClientId > 0)
            .GroupBy(group => group.GroupId)
            .Select(group => group.First())
            .OrderBy(group => group.GroupName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(group => group.GroupId)
            .Select(group => new AnalyticsPowerBiConfigurationGroup(
                group.GroupId,
                group.ClientId,
                group.GroupName.Trim()))
            .ToArray();

        if (!await existsTask)
        {
            return new AnalyticsPowerBiConfigurationResponse(
                optionId,
                false,
                Array.Empty<int>(),
                availableGroups,
                Array.Empty<AnalyticsOptionReportClientEmbed>());
        }

        var groupIdsTask = optionGroupRepository.GetGroupIdsAsync(
            optionId,
            cancellationToken);
        var configurationsTask = configurationService.ResolveAsync(
            optionId,
            cancellationToken);

        await Task.WhenAll(groupIdsTask, configurationsTask);

        var groupIds = (await groupIdsTask)
            .Where(groupId => groupId > 0)
            .Distinct()
            .OrderBy(groupId => groupId)
            .ToArray();
        var clients = (await configurationsTask)
            .Select(AnalyticsReportClientConfigurationContractMapper.Map)
            .ToArray();

        return new AnalyticsPowerBiConfigurationResponse(
            optionId,
            true,
            groupIds,
            availableGroups,
            clients);
    }

    public async Task<AnalyticsAdministrationCommandResult> UpdateAsync(
        int optionId,
        UpdateAnalyticsPowerBiConfigurationRequest request,
        int userId,
        CancellationToken cancellationToken)
    {
        var optionCode = request.OptionCode?.Trim();
        var optionName = request.OptionName?.Trim();

        if (string.IsNullOrWhiteSpace(optionCode) ||
            string.IsNullOrWhiteSpace(optionName))
        {
            return AnalyticsAdministrationCommandResult.Invalid(
                "Datos de opción inválidos",
                "optionCode y optionName son obligatorios.");
        }

        var requestedGroupIds = request.GroupIds ?? [];

        if (requestedGroupIds.Any(groupId => groupId <= 0))
        {
            return AnalyticsAdministrationCommandResult.Invalid(
                "Grupos inválidos",
                "Todos los groupIds deben ser enteros positivos.");
        }

        var groupIds = requestedGroupIds
            .Distinct()
            .OrderBy(groupId => groupId)
            .ToArray();

        if (AnalyticsOptionGroupScopeRules.RequiresExactlyOneGroup(optionId) &&
            groupIds.Length != 1)
        {
            return AnalyticsAdministrationCommandResult.Invalid(
                "Grupo asociado inválido",
                "Gestión Integral de Cobranza debe tener exactamente un grupo SISGES asociado.");
        }

        var previousGroupIdsTask = optionGroupRepository.GetGroupIdsAsync(
            optionId,
            cancellationToken);
        var requestedPublications = request.Publications ?? [];
        Task<IReadOnlyList<AnalyticsReportClientConfiguration>> configurationsTask =
            requestedPublications.Count == 0
                ? Task.FromResult<IReadOnlyList<AnalyticsReportClientConfiguration>>(
                    Array.Empty<AnalyticsReportClientConfiguration>())
                : configurationService.ResolveAsync(
                    optionId,
                    cancellationToken);

        await Task.WhenAll(previousGroupIdsTask, configurationsTask);

        var publicationValidation = AnalyticsReportClientPublicationUpdateValidator.Validate(
            request.Publications,
            await configurationsTask,
            powerBiSecurityPolicy.AllowPublishToWeb);

        if (publicationValidation.Error is not null)
        {
            return AnalyticsAdministrationCommandResult.Invalid(
                publicationValidation.Error.Title,
                publicationValidation.Error.Detail);
        }

        await writer.UpdateAsync(
            optionId,
            optionCode,
            optionName,
            request.IsActive,
            await previousGroupIdsTask,
            groupIds,
            publicationValidation.Updates,
            userId,
            cancellationToken);

        return AnalyticsAdministrationCommandResult.Success();
    }
}
