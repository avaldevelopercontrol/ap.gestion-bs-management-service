using System.Data;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

internal sealed class AnalyticsReportClientPublicationWriter(
    AnalyticsDbContext context,
    IAnalyticsAccessCache cache)
    : IAnalyticsReportClientPublicationWriter
{
    public async Task PatchAsync(
        int optionId,
        IReadOnlyCollection<AnalyticsReportClientPublicationUpdate> publications,
        int updatedBy,
        CancellationToken cancellationToken)
    {
        if (publications.Count == 0)
        {
            return;
        }

        var normalizedPublications = publications
            .Select(publication => new NormalizedPublication(
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

        await using var transaction = await context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);

        var now = DateTime.UtcNow;

        foreach (var publication in normalizedPublications)
        {
            if (publication.GroupIds is not null)
            {
                await ApplyGroupScopesAsync(
                    optionId,
                    publication,
                    updatedBy,
                    now,
                    cancellationToken);
            }

            await ApplyEmbedAsync(
                optionId,
                publication,
                updatedBy,
                now,
                cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        cache.Remove(AnalyticsAccessCacheKeys.ReportClientPublications(optionId));
    }

    private async Task ApplyGroupScopesAsync(
        int optionId,
        NormalizedPublication publication,
        int updatedBy,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var existingScopes = await context.AnalyticsReportClientScopes
            .Where(scope =>
                scope.OptionId == optionId &&
                scope.CrmClientId == publication.ClientId &&
                scope.ReportClientValue == publication.Name)
            .ToListAsync(cancellationToken);

        foreach (var scope in existingScopes.Where(scope => scope.IsActive))
        {
            scope.IsActive = false;
            scope.UpdatedBy = updatedBy;
            scope.UpdatedAt = now;
        }

        foreach (var groupId in publication.GroupIds!)
        {
            var scope = existingScopes.FirstOrDefault(
                item => item.SisgesGroupId == groupId);

            if (scope is null)
            {
                await context.AnalyticsReportClientScopes.AddAsync(
                    new AnalyticsReportClientScopeEntry
                    {
                        OptionId = optionId,
                        CrmClientId = publication.ClientId,
                        ReportClientValue = publication.Name,
                        SisgesGroupId = groupId,
                        IsActive = true,
                        CreatedBy = updatedBy,
                        CreatedAt = now,
                        UpdatedBy = updatedBy,
                        UpdatedAt = now
                    },
                    cancellationToken);
                continue;
            }

            scope.IsActive = true;
            scope.UpdatedBy = updatedBy;
            scope.UpdatedAt = now;
        }
    }

    private async Task ApplyEmbedAsync(
        int optionId,
        NormalizedPublication publication,
        int updatedBy,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var embed = await context.AnalyticsReportClientEmbeds
            .SingleOrDefaultAsync(
                item =>
                    item.OptionId == optionId &&
                    item.CrmClientId == publication.ClientId &&
                    item.ReportClientValue == publication.Name,
                cancellationToken);

        if (publication.EmbedUrl is null)
        {
            if (embed is not null && embed.IsActive)
            {
                embed.IsActive = false;
                embed.UpdatedBy = updatedBy;
                embed.UpdatedAt = now;
            }

            return;
        }

        if (embed is null)
        {
            await context.AnalyticsReportClientEmbeds.AddAsync(
                new AnalyticsReportClientEmbedEntry
                {
                    OptionId = optionId,
                    CrmClientId = publication.ClientId,
                    ReportClientValue = publication.Name,
                    EmbedUrl = publication.EmbedUrl,
                    IsActive = true,
                    CreatedBy = updatedBy,
                    CreatedAt = now,
                    UpdatedBy = updatedBy,
                    UpdatedAt = now
                },
                cancellationToken);
            return;
        }

        embed.EmbedUrl = publication.EmbedUrl;
        embed.IsActive = true;
        embed.UpdatedBy = updatedBy;
        embed.UpdatedAt = now;
    }

    private sealed record NormalizedPublication(
        int ClientId,
        string Name,
        IReadOnlyCollection<int>? GroupIds,
        string? EmbedUrl);
}
