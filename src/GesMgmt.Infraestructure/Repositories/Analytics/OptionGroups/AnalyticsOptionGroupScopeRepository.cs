using System.Data;
using System.Text.Json;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

internal sealed class AnalyticsOptionGroupScopeRepository(
    GesMgmt.Infraestructure.Persistence.AnalyticsDbContext context)
    : IAnalyticsOptionGroupScopeRepository
{
    public Task<bool> HasAnyScopeAsync(
        int optionId,
        CancellationToken cancellationToken) =>
        context.AnalyticsOptionGroupScopes
            .AsNoTracking()
            .AnyAsync(
                scope => scope.OptionId == optionId,
                cancellationToken);

    public async Task<IReadOnlyList<int>> GetGroupIdsAsync(
        int optionId,
        CancellationToken cancellationToken) =>
        await context.AnalyticsOptionGroupScopes
            .AsNoTracking()
            .Where(scope =>
                scope.OptionId == optionId &&
                scope.IsActive)
            .OrderBy(scope => scope.SisgesGroupId)
            .Select(scope => scope.SisgesGroupId)
            .ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<AnalyticsOptionGroupScopeEntry>> GetScopesAsync(
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
            return Array.Empty<AnalyticsOptionGroupScopeEntry>();
        }

        return await context.AnalyticsOptionGroupScopes
            .AsNoTracking()
            .Where(scope => normalizedOptionIds.Contains(scope.OptionId))
            .OrderBy(scope => scope.OptionId)
            .ThenBy(scope => scope.SisgesGroupId)
            .Select(scope => new AnalyticsOptionGroupScopeEntry
            {
                OptionId = scope.OptionId,
                SisgesGroupId = scope.SisgesGroupId,
                IsActive = scope.IsActive
            })
            .ToArrayAsync(cancellationToken);
    }

    public async Task ReplaceAsync(
        int optionId,
        IReadOnlyCollection<int> previousGroupIds,
        IReadOnlyCollection<int> groupIds,
        int? userId,
        CancellationToken cancellationToken)
    {
        var normalizedPreviousGroupIds = Normalize(previousGroupIds);
        var normalizedGroupIds = Normalize(groupIds);
        var requestedGroupIds = normalizedGroupIds.ToHashSet();
        var now = DateTime.UtcNow;

        await using var transaction = await context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);

        var existingScopes = await context.AnalyticsOptionGroupScopes
            .Where(scope => scope.OptionId == optionId)
            .ToListAsync(cancellationToken);

        foreach (var scope in existingScopes)
        {
            if (requestedGroupIds.Contains(scope.SisgesGroupId))
            {
                scope.IsActive = true;
                scope.UpdatedBy = userId;
                scope.UpdatedAt = now;
                requestedGroupIds.Remove(scope.SisgesGroupId);
                continue;
            }

            if (!scope.IsActive)
            {
                continue;
            }

            scope.IsActive = false;
            scope.UpdatedBy = userId;
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
                    CreatedBy = userId,
                    CreatedAt = now,
                    UpdatedBy = userId,
                    UpdatedAt = now
                },
                cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);

        var previousGroupIdsJson = JsonSerializer.Serialize(normalizedPreviousGroupIds);
        var newGroupIdsJson = JsonSerializer.Serialize(normalizedGroupIds);

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
                {userId},
                {now}
            );
            """,
            cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }

    private static int[] Normalize(IReadOnlyCollection<int> groupIds) =>
        groupIds
            .Where(groupId => groupId > 0)
            .Distinct()
            .OrderBy(groupId => groupId)
            .ToArray();
}
