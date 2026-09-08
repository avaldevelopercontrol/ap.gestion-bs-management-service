using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
namespace GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;

public interface IPortfolioPromisesRepository
{
    Task<PortfolioPromisesContext?> ResolveContextAsync(
        int crmClientId,
        string? campaignCode,
        long? subPortfolioId,
        string? businessUnit,
        CancellationToken cancellationToken);

    Task<PortfolioPromisesDbRow> GetOperationalPromisesAsync(
        int clientKey,
        int campaignKey,
        long? subPortfolioId,
        string? businessUnit,
        CancellationToken cancellationToken);
}
