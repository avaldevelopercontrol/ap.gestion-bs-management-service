using System.Data;
using System.Text.Json;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

internal sealed class AnalyticsOptionClientScopeRepository(
    GesMgmt.Infraestructure.Persistence.AnalyticsDbContext context)
    : IAnalyticsOptionClientScopeRepository
{
    public async Task<IReadOnlyList<int>> GetClientIdsAsync(
        int optionId,
        CancellationToken cancellationToken) =>
        await context.AnalyticsOptionClientScopes
            .AsNoTracking()
            .Where(scope =>
                scope.OptionId == optionId &&
                scope.IsActive)
            .OrderBy(scope => scope.CrmClientId)
            .Select(scope => scope.CrmClientId)
            .ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<int>> GetAuthorizedClientIdsAsync(
        int userId,
        int optionId,
        CancellationToken cancellationToken) =>
        await GetAuthorizedScopes(userId, optionId)
            .OrderBy(scope => scope.CrmClientId)
            .Select(scope => scope.CrmClientId)
            .ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<AnalyticsAuthorizedClientScopeEntry>> GetAuthorizedClientsAsync(
        int userId,
        int optionId,
        CancellationToken cancellationToken)
    {
        var rows = await (
            from scope in GetAuthorizedScopes(userId, optionId)
            join client in context.AnalyticsClients.AsNoTracking()
                on scope.CrmClientId equals client.CrmClientId into clients
            from client in clients.DefaultIfEmpty()
            orderby scope.CrmClientId
            select new
            {
                scope.CrmClientId,
                client.ClientName,
                client.ClientCode
            })
            .ToArrayAsync(cancellationToken);

        return rows
            .Select(row => new AnalyticsAuthorizedClientScopeEntry
            {
                CrmClientId = row.CrmClientId,
                Name = ResolveClientName(
                    row.CrmClientId,
                    row.ClientName,
                    row.ClientCode)
            })
            .ToArray();
    }

    public Task<bool> IsAuthorizedClientAsync(
        int userId,
        int optionId,
        int clientId,
        CancellationToken cancellationToken)
    {
        if (userId <= 0 || optionId <= 0 || clientId <= 0)
        {
            return Task.FromResult(false);
        }

        return GetAuthorizedScopes(userId, optionId)
            .AnyAsync(
                scope => scope.CrmClientId == clientId,
                cancellationToken);
    }

    public async Task<IReadOnlyList<AnalyticsOptionClientScopeEntry>> GetActiveScopesAsync(
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
            return Array.Empty<AnalyticsOptionClientScopeEntry>();
        }

        return await (
            from scope in context.AnalyticsOptionClientScopes.AsNoTracking()
            join option in context.AnalyticsOptionConfigs.AsNoTracking()
                on scope.OptionId equals option.OptionId
            where normalizedOptionIds.Contains(scope.OptionId)
                && scope.IsActive
                && option.IsActive
            orderby scope.OptionId, scope.CrmClientId
            select new AnalyticsOptionClientScopeEntry
            {
                OptionId = scope.OptionId,
                CrmClientId = scope.CrmClientId,
                IsActive = true
            })
            .ToArrayAsync(cancellationToken);
    }

    public async Task ReplaceAsync(
        int optionId,
        IReadOnlyCollection<int> previousClientIds,
        IReadOnlyCollection<int> clientIds,
        int? userId,
        CancellationToken cancellationToken)
    {
        var normalizedPreviousClientIds = Normalize(previousClientIds);
        var normalizedClientIds = Normalize(clientIds);
        var requestedClientIds = normalizedClientIds.ToHashSet();
        var now = DateTime.UtcNow;

        await using var transaction = await context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);

        var existingScopes = await context.AnalyticsOptionClientScopes
            .Where(scope => scope.OptionId == optionId)
            .ToListAsync(cancellationToken);

        foreach (var scope in existingScopes)
        {
            if (requestedClientIds.Contains(scope.CrmClientId))
            {
                scope.IsActive = true;
                scope.UpdatedBy = userId;
                scope.UpdatedAt = now;
                requestedClientIds.Remove(scope.CrmClientId);
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

        foreach (var clientId in requestedClientIds.OrderBy(clientId => clientId))
        {
            await context.AnalyticsOptionClientScopes.AddAsync(
                new AnalyticsOptionClientScopeEntry
                {
                    OptionId = optionId,
                    CrmClientId = clientId,
                    IsActive = true,
                    CreatedBy = userId,
                    CreatedAt = now
                },
                cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);

        var previousClientIdsJson = JsonSerializer.Serialize(normalizedPreviousClientIds);
        var newClientIdsJson = JsonSerializer.Serialize(normalizedClientIds);

        await context.Database.ExecuteSqlInterpolatedAsync(
            $"""
            INSERT INTO analytics_access.option_client_scope_audit
            (
                option_id,
                previous_client_ids,
                new_client_ids,
                created_by,
                created_at
            )
            VALUES
            (
                {optionId},
                {previousClientIdsJson},
                {newClientIdsJson},
                {userId},
                {now}
            );
            """,
            cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }

    private IQueryable<AnalyticsOptionClientScopeEntry> GetAuthorizedScopes(
        int userId,
        int optionId)
    {
        var query = context.AnalyticsOptionClientScopes
            .AsNoTracking()
            .Where(scope =>
                scope.OptionId == optionId &&
                scope.IsActive &&
                context.AnalyticsOptionConfigs.Any(option =>
                    option.OptionId == scope.OptionId &&
                    option.IsActive));

        if (UsesInheritedClientAccess(optionId))
        {
            return query;
        }

        return query.Where(scope =>
            context.AnalyticsUserOptionScopes.Any(userScope =>
                userScope.UserId == userId &&
                userScope.OptionId == scope.OptionId &&
                userScope.IsActive));
    }

    private static string ResolveClientName(
        int clientId,
        string? clientName,
        string? clientCode)
    {
        var normalizedName = clientName?.Trim();

        if (!string.IsNullOrWhiteSpace(normalizedName))
        {
            return normalizedName;
        }

        var normalizedCode = clientCode?.Trim();

        return !string.IsNullOrWhiteSpace(normalizedCode)
            ? normalizedCode
            : $"Cartera {clientId}";
    }

    // Portfolio Control Center is an operational module whose data scope is
    // inherited from SISGES user/group/client assignments. Requiring a second
    // per-user row in Analytics duplicates authorization state and does not
    // scale as users are added or moved between groups. Other options retain
    // the explicit user_option_scope behavior.
    private static bool UsesInheritedClientAccess(int optionId) =>
        optionId == AnalyticsOptionIds.PortfolioControlCenter;

    private static int[] Normalize(IReadOnlyCollection<int> clientIds) =>
        clientIds
            .Where(clientId => clientId > 0)
            .Distinct()
            .OrderBy(clientId => clientId)
            .ToArray();
}
