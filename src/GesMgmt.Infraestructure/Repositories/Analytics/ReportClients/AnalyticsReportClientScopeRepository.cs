using System.Data;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

internal sealed class AnalyticsReportClientScopeRepository(
    AnalyticsDbContext context)
    : IAnalyticsReportClientScopeRepository
{
    public Task<bool> HasAnyScopeAsync(
        int optionId,
        CancellationToken cancellationToken) =>
        context.AnalyticsReportClientScopes
            .AsNoTracking()
            .AnyAsync(
                scope =>
                    scope.OptionId == optionId &&
                    scope.IsActive,
                cancellationToken);

    public async Task<IReadOnlyList<AnalyticsReportClientScopeMapping>> GetMappingsAsync(
        int optionId,
        CancellationToken cancellationToken) =>
        await context.AnalyticsReportClientScopes
            .AsNoTracking()
            .Where(scope =>
                scope.OptionId == optionId &&
                scope.IsActive)
            .OrderBy(scope => scope.ReportClientValue)
            .ThenBy(scope => scope.CrmClientId)
            .ThenBy(scope => scope.SisgesGroupId)
            .Select(scope => new AnalyticsReportClientScopeMapping
            {
                CrmClientId = scope.CrmClientId,
                ReportClientValue = scope.ReportClientValue,
                SisgesGroupId = scope.SisgesGroupId
            })
            .ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<int>> GetOptionIdsWithActiveScopeAsync(
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
            return Array.Empty<int>();
        }

        return await context.AnalyticsReportClientScopes
            .AsNoTracking()
            .Where(scope =>
                scope.IsActive &&
                normalizedOptionIds.Contains(scope.OptionId))
            .Select(scope => scope.OptionId)
            .Distinct()
            .OrderBy(optionId => optionId)
            .ToArrayAsync(cancellationToken);
    }

    public async Task ReplaceForClientAsync(
        int optionId,
        int crmClientId,
        string reportClientValue,
        IReadOnlyCollection<int> groupIds,
        int updatedBy,
        CancellationToken cancellationToken)
    {
        var normalizedName = reportClientValue.Trim();
        var normalizedGroupIds = groupIds
            .Where(groupId => groupId > 0)
            .Distinct()
            .OrderBy(groupId => groupId)
            .ToArray();

        await using var transaction = await context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);

        var existingScopes = await context.AnalyticsReportClientScopes
            .Where(scope =>
                scope.OptionId == optionId &&
                scope.CrmClientId == crmClientId &&
                scope.ReportClientValue == normalizedName)
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;

        foreach (var scope in existingScopes.Where(scope => scope.IsActive))
        {
            scope.IsActive = false;
            scope.UpdatedBy = updatedBy;
            scope.UpdatedAt = now;
        }

        foreach (var groupId in normalizedGroupIds)
        {
            var scope = existingScopes.FirstOrDefault(
                item => item.SisgesGroupId == groupId);

            if (scope is null)
            {
                await context.AnalyticsReportClientScopes.AddAsync(
                    new AnalyticsReportClientScopeEntry
                    {
                        OptionId = optionId,
                        CrmClientId = crmClientId,
                        ReportClientValue = normalizedName,
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

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
