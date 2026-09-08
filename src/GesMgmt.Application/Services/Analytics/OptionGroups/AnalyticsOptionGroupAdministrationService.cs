using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Application.Validators.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;

namespace GesMgmt.Application.Services.Analytics;

public sealed class AnalyticsOptionGroupAdministrationService(
    IAnalyticsOptionConfigRepository optionRepository,
    IAnalyticsOptionGroupScopeRepository repository)
    : IAnalyticsOptionGroupAdministrationService
{
    public async Task<AnalyticsOptionGroupsResponse?> GetAsync(
        int optionId,
        CancellationToken cancellationToken)
    {
        if (!await optionRepository.ExistsAsync(optionId, cancellationToken))
        {
            return null;
        }

        var groupIds = await repository.GetGroupIdsAsync(
            optionId,
            cancellationToken);

        return new AnalyticsOptionGroupsResponse(optionId, groupIds);
    }

    public async Task<AnalyticsAdministrationCommandResult> UpdateAsync(
        int optionId,
        IReadOnlyCollection<int>? requestedGroupIds,
        int userId,
        CancellationToken cancellationToken)
    {
        if (!await optionRepository.ExistsAsync(optionId, cancellationToken))
        {
            return AnalyticsAdministrationCommandResult.NotFound();
        }

        var groupIds = requestedGroupIds ?? [];

        if (groupIds.Any(groupId => groupId <= 0))
        {
            return AnalyticsAdministrationCommandResult.Invalid(
                "Grupos inválidos",
                "Todos los groupIds deben ser enteros positivos.");
        }

        var newGroupIds = groupIds
            .Distinct()
            .OrderBy(groupId => groupId)
            .ToArray();

        if (AnalyticsOptionGroupScopeRules.RequiresExactlyOneGroup(optionId) &&
            newGroupIds.Length != 1)
        {
            return AnalyticsAdministrationCommandResult.Invalid(
                "Grupo asociado inválido",
                "Gestión Integral de Cobranza debe tener exactamente un grupo SISGES asociado.");
        }

        var previousGroupIds = await repository.GetGroupIdsAsync(
            optionId,
            cancellationToken);
        var normalizedPreviousGroupIds = previousGroupIds
            .Distinct()
            .OrderBy(groupId => groupId)
            .ToArray();

        if (!normalizedPreviousGroupIds.SequenceEqual(newGroupIds))
        {
            await repository.ReplaceAsync(
                optionId,
                normalizedPreviousGroupIds,
                newGroupIds,
                userId,
                cancellationToken);
        }

        return AnalyticsAdministrationCommandResult.Success();
    }
}
