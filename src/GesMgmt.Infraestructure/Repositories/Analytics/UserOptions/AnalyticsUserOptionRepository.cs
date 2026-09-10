using System.Data;
using System.Text.Json;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

internal sealed class AnalyticsUserOptionRepository(
    GesMgmt.Infraestructure.Persistence.AnalyticsDbContext context)
    : IAnalyticsUserOptionRepository
{
    public Task<bool> HasAccessAsync(
        int userId,
        int optionId,
        CancellationToken cancellationToken) =>
        context.AnalyticsUserOptionScopes
            .AsNoTracking()
            .AnyAsync(
                scope =>
                    scope.UserId == userId &&
                    scope.OptionId == optionId &&
                    scope.IsActive,
                cancellationToken);

    public async Task<IReadOnlyList<AnalyticsUserOption>> GetUserOptionsAsync(
        int userId,
        CancellationToken cancellationToken) =>
        await (
            from scope in context.AnalyticsUserOptionScopes.AsNoTracking()
            join option in context.AnalyticsOptionConfigs.AsNoTracking()
                on scope.OptionId equals option.OptionId
            where scope.UserId == userId &&
                  scope.IsActive &&
                  option.IsActive
            orderby option.OptionId
            select new AnalyticsUserOption(
                option.OptionId,
                option.OptionCode,
                option.OptionName))
            .ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<int>> GetUserIdsAsync(
        int optionId,
        CancellationToken cancellationToken) =>
        await context.AnalyticsUserOptionScopes
            .AsNoTracking()
            .Where(scope =>
                scope.OptionId == optionId &&
                scope.IsActive)
            .OrderBy(scope => scope.UserId)
            .Select(scope => scope.UserId)
            .ToArrayAsync(cancellationToken);

    public async Task ReplaceAsync(
        int optionId,
        IReadOnlyCollection<int> previousUserIds,
        IReadOnlyCollection<int> userIds,
        int? adminUserId,
        CancellationToken cancellationToken)
    {
        var normalizedPreviousUserIds = Normalize(previousUserIds);
        var normalizedUserIds = Normalize(userIds);
        var requestedUserIds = normalizedUserIds.ToHashSet();
        var now = DateTime.UtcNow;

        await using var transaction = await context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);

        var existingScopes = await context.AnalyticsUserOptionScopes
            .Where(scope => scope.OptionId == optionId)
            .ToListAsync(cancellationToken);

        foreach (var scope in existingScopes)
        {
            if (requestedUserIds.Contains(scope.UserId))
            {
                scope.IsActive = true;
                scope.UpdatedBy = adminUserId;
                scope.UpdatedAt = now;
                requestedUserIds.Remove(scope.UserId);
                continue;
            }

            if (!scope.IsActive)
            {
                continue;
            }

            scope.IsActive = false;
            scope.UpdatedBy = adminUserId;
            scope.UpdatedAt = now;
        }

        foreach (var userId in requestedUserIds.OrderBy(userId => userId))
        {
            await context.AnalyticsUserOptionScopes.AddAsync(
                new AnalyticsUserOptionScopeEntry
                {
                    UserId = userId,
                    OptionId = optionId,
                    IsActive = true,
                    CreatedBy = adminUserId,
                    CreatedAt = now
                },
                cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);

        var previousUserIdsJson = JsonSerializer.Serialize(normalizedPreviousUserIds);
        var newUserIdsJson = JsonSerializer.Serialize(normalizedUserIds);

        await context.Database.ExecuteSqlInterpolatedAsync(
            $"""
            INSERT INTO analytics_access.user_option_scope_audit
            (
                option_id,
                previous_user_ids,
                new_user_ids,
                created_by,
                created_at
            )
            VALUES
            (
                {optionId},
                {previousUserIdsJson},
                {newUserIdsJson},
                {adminUserId},
                {now}
            );
            """,
            cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }

    private static int[] Normalize(IReadOnlyCollection<int> userIds) =>
        userIds
            .Where(userId => userId > 0)
            .Distinct()
            .OrderBy(userId => userId)
            .ToArray();
}
