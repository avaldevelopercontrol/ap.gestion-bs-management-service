using System.Data;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

internal sealed class AnalyticsReportClientEmbedRepository(
    AnalyticsDbContext context,
    IAnalyticsAccessCache cache)
    : IAnalyticsReportClientEmbedRepository
{
    public async Task<AnalyticsReportClientEmbedMapping?> GetAsync(
        int optionId,
        int crmClientId,
        string reportClientValue,
        CancellationToken cancellationToken)
    {
        var normalizedName = reportClientValue.Trim();
        var publications = await GetActiveForOptionAsync(
            optionId,
            cancellationToken);

        var publication = publications.FirstOrDefault(item =>
            item.CrmClientId == crmClientId &&
            string.Equals(
                item.ReportClientValue,
                normalizedName,
                StringComparison.OrdinalIgnoreCase));

        return publication is null
            ? null
            : new AnalyticsReportClientEmbedMapping
            {
                ReportClientValue = publication.ReportClientValue,
                EmbedUrl = publication.EmbedUrl ?? string.Empty
            };
    }

    public Task<IReadOnlyList<AnalyticsReportClientEmbedAdminMapping>> GetActiveForOptionAsync(
        int optionId,
        CancellationToken cancellationToken) =>
        cache.GetOrCreateAsync<IReadOnlyList<AnalyticsReportClientEmbedAdminMapping>>(
            AnalyticsAccessCacheKeys.ReportClientPublications(optionId),
            AnalyticsAccessCachePolicy.ConfigurationDuration,
            async token => await context.AnalyticsReportClientEmbeds
                .AsNoTracking()
                .Where(embed =>
                    embed.OptionId == optionId &&
                    embed.IsActive)
                .OrderBy(embed => embed.ReportClientValue)
                .ThenBy(embed => embed.CrmClientId)
                .Select(embed => new AnalyticsReportClientEmbedAdminMapping
                {
                    CrmClientId = embed.CrmClientId,
                    ReportClientValue = embed.ReportClientValue,
                    EmbedUrl = embed.EmbedUrl
                })
                .ToArrayAsync(token),
            cancellationToken);

    public async Task UpsertAsync(
        int optionId,
        int crmClientId,
        string reportClientValue,
        string embedUrl,
        int updatedBy,
        CancellationToken cancellationToken)
    {
        var normalizedName = reportClientValue.Trim();
        var normalizedEmbedUrl = embedUrl.Trim();

        await using var transaction = await context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);

        var entry = await context.AnalyticsReportClientEmbeds
            .SingleOrDefaultAsync(
                embed =>
                    embed.OptionId == optionId &&
                    embed.CrmClientId == crmClientId &&
                    embed.ReportClientValue == normalizedName,
                cancellationToken);

        var now = DateTime.UtcNow;

        if (entry is null)
        {
            await context.AnalyticsReportClientEmbeds.AddAsync(
                new AnalyticsReportClientEmbedEntry
                {
                    OptionId = optionId,
                    CrmClientId = crmClientId,
                    ReportClientValue = normalizedName,
                    EmbedUrl = normalizedEmbedUrl,
                    IsActive = true,
                    CreatedBy = updatedBy,
                    CreatedAt = now,
                    UpdatedBy = updatedBy,
                    UpdatedAt = now
                },
                cancellationToken);
        }
        else
        {
            entry.EmbedUrl = normalizedEmbedUrl;
            entry.IsActive = true;
            entry.UpdatedBy = updatedBy;
            entry.UpdatedAt = now;
        }

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        cache.Remove(AnalyticsAccessCacheKeys.ReportClientPublications(optionId));
    }

    public async Task DeactivateAsync(
        int optionId,
        int crmClientId,
        string reportClientValue,
        int updatedBy,
        CancellationToken cancellationToken)
    {
        var normalizedName = reportClientValue.Trim();
        var entry = await context.AnalyticsReportClientEmbeds
            .SingleOrDefaultAsync(
                embed =>
                    embed.OptionId == optionId &&
                    embed.CrmClientId == crmClientId &&
                    embed.ReportClientValue == normalizedName &&
                    embed.IsActive,
                cancellationToken);

        if (entry is not null)
        {
            entry.IsActive = false;
            entry.UpdatedBy = updatedBy;
            entry.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync(cancellationToken);
        }

        cache.Remove(AnalyticsAccessCacheKeys.ReportClientPublications(optionId));
    }
}
