using System.Data;
using System.Text.Json;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

internal sealed class AnalyticsPowerBiConfigurationWriter(
    AnalyticsDbContext context,
    IAnalyticsAccessCache cache)
    : IAnalyticsPowerBiConfigurationWriter
{
    public async Task UpdateAsync(
        int optionId,
        string optionCode,
        string optionName,
        bool isActive,
        IReadOnlyCollection<int> previousGroupIds,
        IReadOnlyCollection<int> groupIds,
        IReadOnlyCollection<AnalyticsReportClientPublicationUpdate> publications,
        int updatedBy,
        CancellationToken cancellationToken)
    {
        var normalizedPreviousGroupIds = NormalizeGroupIds(previousGroupIds);
        var normalizedGroupIds = NormalizeGroupIds(groupIds);
        var normalizedPublications = NormalizePublications(publications);
        var groupsChanged = !normalizedPreviousGroupIds.SequenceEqual(normalizedGroupIds);
        var now = DateTime.UtcNow;

        await using var transaction = await context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);

        await UpsertOptionAsync(
            optionId,
            optionCode.Trim(),
            optionName.Trim(),
            isActive,
            updatedBy,
            now,
            cancellationToken);

        if (groupsChanged)
        {
            await ReplaceOptionGroupsAsync(
                optionId,
                normalizedGroupIds,
                updatedBy,
                now,
                cancellationToken);
        }

        if (normalizedPublications.Length > 0)
        {
            await ApplyPublicationsAsync(
                optionId,
                normalizedPublications,
                updatedBy,
                now,
                cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);

        if (groupsChanged)
        {
            await InsertGroupAuditAsync(
                optionId,
                normalizedPreviousGroupIds,
                normalizedGroupIds,
                updatedBy,
                now,
                cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);

        cache.Remove(AnalyticsAccessCacheKeys.ActiveOptions);

        if (normalizedPublications.Length > 0)
        {
            cache.Remove(AnalyticsAccessCacheKeys.ReportClientPublications(optionId));
        }
    }

    private async Task UpsertOptionAsync(
        int optionId,
        string optionCode,
        string optionName,
        bool isActive,
        int updatedBy,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var option = await context.AnalyticsOptionConfigs
            .SingleOrDefaultAsync(
                current => current.OptionId == optionId,
                cancellationToken);

        if (option is null)
        {
            await context.AnalyticsOptionConfigs.AddAsync(
                new AnalyticsOptionConfig
                {
                    OptionId = optionId,
                    OptionCode = optionCode,
                    OptionName = optionName,
                    IsActive = isActive,
                    CreatedBy = updatedBy,
                    CreatedAt = now
                },
                cancellationToken);
            return;
        }

        option.OptionCode = optionCode;
        option.OptionName = optionName;
        option.IsActive = isActive;
        option.UpdatedBy = updatedBy;
        option.UpdatedAt = now;
    }

    private async Task ReplaceOptionGroupsAsync(
        int optionId,
        IReadOnlyCollection<int> groupIds,
        int updatedBy,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var requestedGroupIds = groupIds.ToHashSet();
        var existingScopes = await context.AnalyticsOptionGroupScopes
            .Where(scope => scope.OptionId == optionId)
            .ToListAsync(cancellationToken);

        foreach (var scope in existingScopes)
        {
            if (requestedGroupIds.Remove(scope.SisgesGroupId))
            {
                scope.IsActive = true;
                scope.UpdatedBy = updatedBy;
                scope.UpdatedAt = now;
                continue;
            }

            if (!scope.IsActive)
            {
                continue;
            }

            scope.IsActive = false;
            scope.UpdatedBy = updatedBy;
            scope.UpdatedAt = now;
        }

        foreach (var groupId in requestedGroupIds.OrderBy(groupId => groupId))
        {
            await context.AnalyticsOptionGroupScopes.AddAsync(
                new AnalyticsOptionGroupScopeEntry
                {
                    OptionId = optionId,
                    SisgesGroupId = groupId,
                    IsActive = true,
                    CreatedBy = updatedBy,
                    CreatedAt = now,
                    UpdatedBy = updatedBy,
                    UpdatedAt = now
                },
                cancellationToken);
        }
    }

    private async Task InsertGroupAuditAsync(
        int optionId,
        IReadOnlyCollection<int> previousGroupIds,
        IReadOnlyCollection<int> groupIds,
        int updatedBy,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var previousGroupIdsJson = JsonSerializer.Serialize(previousGroupIds);
        var newGroupIdsJson = JsonSerializer.Serialize(groupIds);

        await context.Database.ExecuteSqlInterpolatedAsync(
            $"""
            INSERT INTO analytics_access.option_group_scope_audit
            (
                option_id,
                previous_group_ids,
                new_group_ids,
                created_by,
                created_at
            )
            VALUES
            (
                {optionId},
                {previousGroupIdsJson},
                {newGroupIdsJson},
                {updatedBy},
                {now}
            );
            """,
            cancellationToken);
    }

    private async Task ApplyPublicationsAsync(
        int optionId,
        IReadOnlyCollection<NormalizedPublication> publications,
        int updatedBy,
        DateTime now,
        CancellationToken cancellationToken)
    {
        foreach (var publication in publications)
        {
            if (publication.GroupIds is not null)
            {
                await ApplyReportClientGroupsAsync(
                    optionId,
                    publication,
                    updatedBy,
                    now,
                    cancellationToken);
            }

            await ApplyReportClientEmbedAsync(
                optionId,
                publication,
                updatedBy,
                now,
                cancellationToken);
        }
    }

    private async Task ApplyReportClientGroupsAsync(
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

    private async Task ApplyReportClientEmbedAsync(
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

    private static int[] NormalizeGroupIds(
        IEnumerable<int> groupIds) =>
        groupIds
            .Where(groupId => groupId > 0)
            .Distinct()
            .OrderBy(groupId => groupId)
            .ToArray();

    private static NormalizedPublication[] NormalizePublications(
        IEnumerable<AnalyticsReportClientPublicationUpdate> publications) =>
        publications
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

    private sealed record NormalizedPublication(
        int ClientId,
        string Name,
        IReadOnlyCollection<int>? GroupIds,
        string? EmbedUrl);
}
