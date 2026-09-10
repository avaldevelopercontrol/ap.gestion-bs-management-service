using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Domain.Interfaces.Analytics;

public interface IAnalyticsReportClientEmbedRepository
{
    Task<AnalyticsReportClientEmbedMapping?> GetAsync(
        int optionId,
        int crmClientId,
        string reportClientValue,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<AnalyticsReportClientEmbedAdminMapping>> GetActiveForOptionAsync(
        int optionId,
        CancellationToken cancellationToken);

    Task UpsertAsync(
        int optionId,
        int crmClientId,
        string reportClientValue,
        string embedUrl,
        int updatedBy,
        CancellationToken cancellationToken);

    Task DeactivateAsync(
        int optionId,
        int crmClientId,
        string reportClientValue,
        int updatedBy,
        CancellationToken cancellationToken);
}
