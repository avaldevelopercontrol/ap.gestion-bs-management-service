using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Application.Validators.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;

namespace GesMgmt.Application.Services.Analytics;

public sealed class AnalyticsReportClientConfigurationService(
    IAnalyticsReportClientCatalogRepository catalogRepository,
    IAnalyticsReportClientScopeRepository scopeRepository,
    IAnalyticsReportClientEmbedRepository embedRepository,
    ISisgesClientGroupRepository clientGroupRepository,
    IAnalyticsPowerBiSecurityPolicy powerBiSecurityPolicy)
    : IAnalyticsReportClientConfigurationService
{
    public async Task<bool> RequiresClientSelectionAsync(
        int optionId,
        CancellationToken cancellationToken)
    {
        if (optionId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(optionId));
        }

        if (catalogRepository.Supports(optionId))
        {
            return true;
        }

        return await scopeRepository.HasAnyScopeAsync(
            optionId,
            cancellationToken);
    }

    public async Task<IReadOnlyDictionary<int, bool>> RequiresClientSelectionManyAsync(
        IReadOnlyCollection<int> optionIds,
        CancellationToken cancellationToken)
    {
        var normalizedOptionIds = optionIds
            .Where(optionId => optionId > 0)
            .Distinct()
            .OrderBy(optionId => optionId)
            .ToArray();

        if (normalizedOptionIds.Length == 0)
        {
            return new Dictionary<int, bool>();
        }

        var result = normalizedOptionIds
            .ToDictionary(
                optionId => optionId,
                catalogRepository.Supports);
        var scopeBackedOptionIds = result
            .Where(pair => !pair.Value)
            .Select(pair => pair.Key)
            .ToArray();

        if (scopeBackedOptionIds.Length == 0)
        {
            return result;
        }

        var optionIdsWithActiveScope = await scopeRepository
            .GetOptionIdsWithActiveScopeAsync(
                scopeBackedOptionIds,
                cancellationToken);

        foreach (var optionId in optionIdsWithActiveScope)
        {
            if (result.ContainsKey(optionId))
            {
                result[optionId] = true;
            }
        }

        return result;
    }

    public async Task<IReadOnlyList<AnalyticsReportClientConfiguration>> ResolveAsync(
        int optionId,
        CancellationToken cancellationToken)
    {
        if (optionId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(optionId));
        }

        var usesLiveCatalog = catalogRepository.Supports(optionId);

        IReadOnlyList<AnalyticsReportClientCatalogItem> catalogItems =
            usesLiveCatalog
                ? await catalogRepository.GetCurrentAsync(
                    optionId,
                    cancellationToken)
                : Array.Empty<AnalyticsReportClientCatalogItem>();

        var scopeMappings = await scopeRepository.GetMappingsAsync(
            optionId,
            cancellationToken);
        var publications = await embedRepository.GetActiveForOptionAsync(
            optionId,
            cancellationToken);

        var clientIds = catalogItems
            .Select(item => item.CrmClientId)
            .Concat(scopeMappings.Select(mapping => mapping.CrmClientId))
            .Concat(publications.Select(publication => publication.CrmClientId))
            .Where(clientId => clientId > 0)
            .Distinct()
            .ToArray();

        var clientGroups = await clientGroupRepository.GetActiveGroupsAsync(
            clientIds,
            cancellationToken);

        return AnalyticsReportClientConfigurationResolver.Resolve(
            usesLiveCatalog,
            catalogItems,
            scopeMappings,
            publications,
            clientGroups,
            powerBiSecurityPolicy.AllowPublishToWeb);
    }
}
