using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Application.Validators.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;

namespace GesMgmt.Application.Services.Analytics;

public sealed class AnalyticsOptionClientAdministrationService(
    IAnalyticsOptionConfigRepository optionRepository,
    IAnalyticsOptionClientScopeRepository repository)
    : IAnalyticsOptionClientAdministrationService
{
    public async Task<AnalyticsOptionClientsResponse?> GetAsync(
        int optionId,
        CancellationToken cancellationToken)
    {
        if (!await optionRepository.ExistsAsync(optionId, cancellationToken))
        {
            return null;
        }

        var clientIds = await repository.GetClientIdsAsync(
            optionId,
            cancellationToken);

        return new AnalyticsOptionClientsResponse(optionId, clientIds);
    }

    public async Task<AnalyticsAdministrationCommandResult> UpdateAsync(
        int optionId,
        IReadOnlyCollection<int>? requestedClientIds,
        int userId,
        CancellationToken cancellationToken)
    {
        if (!await optionRepository.ExistsAsync(optionId, cancellationToken))
        {
            return AnalyticsAdministrationCommandResult.NotFound();
        }

        var clientIds = requestedClientIds ?? [];

        if (clientIds.Any(clientId => clientId <= 0))
        {
            return AnalyticsAdministrationCommandResult.Invalid(
                "Clientes inválidos",
                "Todos los clientIds deben ser enteros positivos.");
        }

        var newClientIds = clientIds
            .Distinct()
            .OrderBy(clientId => clientId)
            .ToArray();
        var previousClientIds = await repository.GetClientIdsAsync(
            optionId,
            cancellationToken);
        var normalizedPreviousClientIds = previousClientIds
            .Distinct()
            .OrderBy(clientId => clientId)
            .ToArray();

        if (!normalizedPreviousClientIds.SequenceEqual(newClientIds))
        {
            await repository.ReplaceAsync(
                optionId,
                normalizedPreviousClientIds,
                newClientIds,
                userId,
                cancellationToken);
        }

        return AnalyticsAdministrationCommandResult.Success();
    }
}
