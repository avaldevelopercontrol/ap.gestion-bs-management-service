using System.Text.Json;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence.Analytics;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

internal static class AnalyticsReportClientPublicationPatch
{
    public static AnalyticsDbCommand CreateCommand(
        int optionId,
        IReadOnlyCollection<AnalyticsReportClientPublicationUpdate> publications,
        int updatedBy)
    {
        var normalizedPublications = publications
            .Select(publication => new PublicationPatchItem(
                publication.ClientId,
                publication.Name.Trim(),
                publication.GroupIds is null
                    ? null
                    : publication.GroupIds
                        .Where(groupId => groupId > 0)
                        .Distinct()
                        .OrderBy(groupId => groupId)
                        .ToArray(),
                string.IsNullOrWhiteSpace(publication.EmbedUrl)
                    ? null
                    : publication.EmbedUrl.Trim()))
            .ToArray();

        return new AnalyticsDbCommand(
            AnalyticsReportClientPublicationSql.ApplyPatch,
            new AnalyticsReportClientPublicationPatchParameters(
                optionId,
                JsonSerializer.Serialize(normalizedPublications),
                normalizedPublications.Any(publication => publication.GroupIds is not null),
                updatedBy));
    }

    private sealed record PublicationPatchItem(
        int ClientId,
        string Name,
        IReadOnlyCollection<int>? GroupIds,
        string? EmbedUrl);
}

public sealed record AnalyticsReportClientPublicationPatchParameters(
    int OptionId,
    string PublicationsJson,
    bool HasGroupUpdates,
    int UpdatedBy);
