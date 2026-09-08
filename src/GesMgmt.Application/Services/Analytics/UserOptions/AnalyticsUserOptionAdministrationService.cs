using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Application.Validators.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;

namespace GesMgmt.Application.Services.Analytics;

public sealed class AnalyticsUserOptionAdministrationService(
    IAnalyticsUserOptionRepository repository,
    IAnalyticsOptionConfigRepository optionRepository)
    : IAnalyticsUserOptionAdministrationService
{
    public async Task<AnalyticsOptionUsersResponse?> GetAsync(
        int optionId,
        CancellationToken cancellationToken)
    {
        if (!await optionRepository.ExistsAsync(optionId, cancellationToken))
        {
            return null;
        }

        var userIds = await repository.GetUserIdsAsync(
            optionId,
            cancellationToken);

        return new AnalyticsOptionUsersResponse(optionId, userIds);
    }

    public async Task<AnalyticsAdministrationCommandResult> UpdateAsync(
        int optionId,
        IReadOnlyCollection<int>? requestedUserIds,
        int adminUserId,
        CancellationToken cancellationToken)
    {
        if (!await optionRepository.ExistsAsync(optionId, cancellationToken))
        {
            return AnalyticsAdministrationCommandResult.NotFound();
        }

        var userIds = requestedUserIds ?? [];

        if (userIds.Any(userId => userId <= 0))
        {
            return AnalyticsAdministrationCommandResult.Invalid(
                "Usuarios inválidos",
                "Todos los userIds deben ser enteros positivos.");
        }

        var newUserIds = userIds
            .Distinct()
            .OrderBy(userId => userId)
            .ToArray();
        var previousUserIds = await repository.GetUserIdsAsync(
            optionId,
            cancellationToken);
        var normalizedPreviousUserIds = previousUserIds
            .Distinct()
            .OrderBy(userId => userId)
            .ToArray();

        if (!normalizedPreviousUserIds.SequenceEqual(newUserIds))
        {
            await repository.ReplaceAsync(
                optionId,
                normalizedPreviousUserIds,
                newUserIds,
                adminUserId,
                cancellationToken);
        }

        return AnalyticsAdministrationCommandResult.Success();
    }
}
