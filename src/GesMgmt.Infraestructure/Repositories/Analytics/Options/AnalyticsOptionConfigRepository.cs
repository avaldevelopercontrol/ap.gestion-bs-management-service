using System.Data;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

internal sealed class AnalyticsOptionConfigRepository(
    AnalyticsDbContext context,
    IAnalyticsAccessCache cache)
    : IAnalyticsOptionConfigRepository
{
    public Task<bool> ExistsAsync(
        int optionId,
        CancellationToken cancellationToken)
    {
        if (optionId <= 0)
        {
            return Task.FromResult(false);
        }

        return context.AnalyticsOptionConfigs
            .AsNoTracking()
            .AnyAsync(
                option => option.OptionId == optionId,
                cancellationToken);
    }

    public async Task<bool> IsActiveAsync(
        int optionId,
        CancellationToken cancellationToken)
    {
        if (optionId <= 0)
        {
            return false;
        }

        var options = await GetActiveOptionsAsync(cancellationToken);
        return options.Any(option => option.OptionId == optionId);
    }

    public Task<IReadOnlyList<AnalyticsOptionConfig>> GetAllAsync(
        CancellationToken cancellationToken) =>
        GetActiveOptionsAsync(cancellationToken);

    public async Task UpsertAsync(
        int optionId,
        string optionCode,
        string optionName,
        bool isActive,
        int? userId,
        CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);

        var option = await context.AnalyticsOptionConfigs
            .SingleOrDefaultAsync(
                current => current.OptionId == optionId,
                cancellationToken);

        var now = DateTime.UtcNow;

        if (option is null)
        {
            option = new AnalyticsOptionConfig
            {
                OptionId = optionId,
                OptionCode = optionCode,
                OptionName = optionName,
                IsActive = isActive,
                CreatedBy = userId,
                CreatedAt = now
            };

            await context.AnalyticsOptionConfigs.AddAsync(
                option,
                cancellationToken);
        }
        else
        {
            option.OptionCode = optionCode;
            option.OptionName = optionName;
            option.IsActive = isActive;
            option.UpdatedBy = userId;
            option.UpdatedAt = now;
        }

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        cache.Remove(AnalyticsAccessCacheKeys.ActiveOptions);
    }

    private Task<IReadOnlyList<AnalyticsOptionConfig>> GetActiveOptionsAsync(
        CancellationToken cancellationToken) =>
        cache.GetOrCreateAsync<IReadOnlyList<AnalyticsOptionConfig>>(
            AnalyticsAccessCacheKeys.ActiveOptions,
            AnalyticsAccessCachePolicy.ConfigurationDuration,
            async token => await context.AnalyticsOptionConfigs
                .AsNoTracking()
                .Where(option => option.IsActive)
                .OrderBy(option => option.OptionId)
                .Select(option => new AnalyticsOptionConfig
                {
                    OptionId = option.OptionId,
                    OptionCode = option.OptionCode,
                    OptionName = option.OptionName,
                    IsActive = true
                })
                .ToArrayAsync(token),
            cancellationToken);
}
