using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Application.Validators.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;

namespace GesMgmt.Application.Services.Analytics;

public static class AnalyticsReportClientConfigurationResolver
{
    public static IReadOnlyList<AnalyticsReportClientConfiguration> Resolve(
        bool usesLiveCatalog,
        IReadOnlyList<AnalyticsReportClientCatalogItem> catalogItems,
        IReadOnlyList<AnalyticsReportClientScopeMapping> scopeMappings,
        IReadOnlyList<AnalyticsReportClientEmbedAdminMapping> publications,
        IReadOnlyList<SisgesClientGroup> clientGroups,
        bool allowPublishToWeb = true)
    {
        var liveKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var scopeKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var values = new Dictionary<string, (int ClientId, string Name)>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var item in catalogItems)
        {
            AddValue(
                liveKeys,
                values,
                item.CrmClientId,
                item.ReportClientValue);
        }

        foreach (var mapping in scopeMappings)
        {
            AddValue(
                scopeKeys,
                values,
                mapping.CrmClientId,
                mapping.ReportClientValue);
        }

        foreach (var publication in publications)
        {
            AddValue(
                null,
                values,
                publication.CrmClientId,
                publication.ReportClientValue);
        }

        var candidateGroupsByClient = clientGroups
            .Where(group => group.GroupId > 0 && group.ClientId > 0)
            .GroupBy(group => group.ClientId)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<AnalyticsReportClientConfigurationGroup>)group
                    .GroupBy(candidate => candidate.GroupId)
                    .Select(candidateGroup => candidateGroup.First())
                    .Select(candidate => new AnalyticsReportClientConfigurationGroup(
                        candidate.GroupId,
                        NormalizeGroupName(candidate.GroupName, candidate.GroupId)))
                    .OrderBy(candidate => candidate.Name, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(candidate => candidate.GroupId)
                    .ToArray());

        var explicitGroupsByKey = scopeMappings
            .Where(mapping =>
                mapping.CrmClientId > 0 &&
                mapping.SisgesGroupId > 0 &&
                !string.IsNullOrWhiteSpace(mapping.ReportClientValue))
            .GroupBy(
                mapping => BuildKey(mapping.CrmClientId, mapping.ReportClientValue),
                StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<int>)group
                    .Select(mapping => mapping.SisgesGroupId)
                    .Distinct()
                    .OrderBy(groupId => groupId)
                    .ToArray(),
                StringComparer.OrdinalIgnoreCase);

        var publicationByKey = publications
            .Where(publication =>
                publication.CrmClientId > 0 &&
                !string.IsNullOrWhiteSpace(publication.ReportClientValue) &&
                !string.IsNullOrWhiteSpace(publication.EmbedUrl))
            .GroupBy(
                publication => BuildKey(
                    publication.CrmClientId,
                    publication.ReportClientValue),
                StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => group.First().EmbedUrl?.Trim(),
                StringComparer.OrdinalIgnoreCase);

        var result = new List<AnalyticsReportClientConfiguration>(values.Count);

        foreach (var (key, value) in values)
        {
            var isAvailable = usesLiveCatalog
                ? liveKeys.Contains(key)
                : scopeKeys.Contains(key) || publicationByKey.ContainsKey(key);

            candidateGroupsByClient.TryGetValue(
                value.ClientId,
                out var candidateGroups);

            candidateGroups ??= Array.Empty<AnalyticsReportClientConfigurationGroup>();

            explicitGroupsByKey.TryGetValue(
                key,
                out var explicitGroupIds);

            explicitGroupIds ??= Array.Empty<int>();

            var hasExplicitGroupConfiguration = explicitGroupIds.Count > 0;
            var candidateGroupIds = candidateGroups
                .Select(group => group.GroupId)
                .ToHashSet();

            string groupResolution;
            IReadOnlyList<int> effectiveGroupIds;

            if (!isAvailable)
            {
                groupResolution = AnalyticsReportClientGroupResolution.Unavailable;
                effectiveGroupIds = explicitGroupIds;
            }
            else if (hasExplicitGroupConfiguration)
            {
                var configuredGroupsAreValid = explicitGroupIds
                    .All(candidateGroupIds.Contains);

                groupResolution = configuredGroupsAreValid
                    ? AnalyticsReportClientGroupResolution.Configured
                    : AnalyticsReportClientGroupResolution.InvalidConfigured;

                effectiveGroupIds = configuredGroupsAreValid
                    ? explicitGroupIds
                    : Array.Empty<int>();
            }
            else if (candidateGroups.Count == 1)
            {
                groupResolution = AnalyticsReportClientGroupResolution.AutoDetected;
                effectiveGroupIds = [candidateGroups[0].GroupId];
            }
            else if (candidateGroups.Count == 0)
            {
                groupResolution = AnalyticsReportClientGroupResolution.Missing;
                effectiveGroupIds = Array.Empty<int>();
            }
            else
            {
                groupResolution = AnalyticsReportClientGroupResolution.Ambiguous;
                effectiveGroupIds = Array.Empty<int>();
            }

            publicationByKey.TryGetValue(key, out var embedUrl);

            var hasValidPublication =
                allowPublishToWeb &&
                PowerBiPublishToWebUrl.TryNormalize(
                    embedUrl,
                    out _);

            var isReady =
                isAvailable &&
                AnalyticsReportClientGroupResolution.IsResolved(groupResolution) &&
                hasValidPublication;

            result.Add(
                new AnalyticsReportClientConfiguration(
                    value.ClientId,
                    value.Name,
                    isAvailable,
                    groupResolution,
                    hasExplicitGroupConfiguration,
                    effectiveGroupIds,
                    candidateGroups,
                    embedUrl,
                    isReady));
        }

        return result
            .OrderByDescending(item => item.IsAvailable)
            .ThenBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(item => item.ClientId)
            .ToArray();
    }

    private static void AddValue(
        HashSet<string>? target,
        IDictionary<string, (int ClientId, string Name)> values,
        int clientId,
        string? name)
    {
        var normalizedName = name?.Trim() ?? string.Empty;

        if (clientId <= 0 || normalizedName.Length == 0)
        {
            return;
        }

        var key = BuildKey(clientId, normalizedName);
        target?.Add(key);

        if (!values.ContainsKey(key))
        {
            values[key] = (clientId, normalizedName);
        }
    }

    public static string BuildKey(int clientId, string name) =>
        $"{clientId}:{name.Trim()}";

    private static string NormalizeGroupName(string? value, int groupId)
    {
        var normalized = value?.Trim();
        return string.IsNullOrWhiteSpace(normalized)
            ? $"Grupo {groupId}"
            : normalized;
    }
}
