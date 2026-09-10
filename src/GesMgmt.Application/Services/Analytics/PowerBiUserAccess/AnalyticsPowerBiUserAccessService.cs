using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Application.Validators.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;

namespace GesMgmt.Application.Services.Analytics;

public sealed class AnalyticsPowerBiUserAccessService(
    IAnalyticsOptionConfigRepository optionRepository,
    IAnalyticsOptionAccessService optionAccessService,
    IAnalyticsReportClientConfigurationService reportClientConfigurationService)
    : IAnalyticsPowerBiUserAccessService
{
    public async Task<IReadOnlyList<AnalyticsPowerBiOptionAccess>> ResolveAsync(
        int userId,
        IReadOnlyCollection<int> optionIds,
        CancellationToken cancellationToken)
    {
        if (userId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(userId));
        }

        var normalizedOptionIds = optionIds
            .Where(optionId => optionId > 0)
            .Distinct()
            .OrderBy(optionId => optionId)
            .ToArray();

        if (normalizedOptionIds.Length == 0)
        {
            return Array.Empty<AnalyticsPowerBiOptionAccess>();
        }

        var activeOptions = await optionRepository.GetAllAsync(cancellationToken);
        var activeOptionIds = activeOptions
            .Select(option => option.OptionId)
            .ToHashSet();
        var requestedActiveOptionIds = normalizedOptionIds
            .Where(activeOptionIds.Contains)
            .ToArray();

        IReadOnlyDictionary<int, AnalyticsOptionAccessResult> accessByOption =
            requestedActiveOptionIds.Length == 0
                ? new Dictionary<int, AnalyticsOptionAccessResult>()
                : await optionAccessService.ResolveManyAsync(
                    userId,
                    requestedActiveOptionIds,
                    cancellationToken);

        var allowedOptionIds = requestedActiveOptionIds
            .Where(optionId =>
                accessByOption.TryGetValue(optionId, out var access) &&
                access.Allowed)
            .ToArray();
        IReadOnlyDictionary<int, bool> requiresClientSelectionByOption =
            allowedOptionIds.Length == 0
                ? new Dictionary<int, bool>()
                : await reportClientConfigurationService
                    .RequiresClientSelectionManyAsync(
                        allowedOptionIds,
                        cancellationToken);

        return normalizedOptionIds
            .Select(optionId => ResolveOption(
                optionId,
                activeOptionIds.Contains(optionId),
                accessByOption,
                requiresClientSelectionByOption))
            .ToArray();
    }

    private static AnalyticsPowerBiOptionAccess ResolveOption(
        int optionId,
        bool isActive,
        IReadOnlyDictionary<int, AnalyticsOptionAccessResult> accessByOption,
        IReadOnlyDictionary<int, bool> requiresClientSelectionByOption)
    {
        if (!isActive ||
            !accessByOption.TryGetValue(optionId, out var access) ||
            !access.Allowed ||
            !requiresClientSelectionByOption.TryGetValue(
                optionId,
                out var requiresClientSelection))
        {
            return new AnalyticsPowerBiOptionAccess(
                optionId,
                Allowed: false,
                RequiresClientSelection: false);
        }

        return new AnalyticsPowerBiOptionAccess(
            optionId,
            Allowed: true,
            RequiresClientSelection: requiresClientSelection);
    }
}
