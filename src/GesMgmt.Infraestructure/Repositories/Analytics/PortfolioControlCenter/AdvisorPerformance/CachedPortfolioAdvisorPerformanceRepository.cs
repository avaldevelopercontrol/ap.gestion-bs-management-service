using Microsoft.Extensions.DependencyInjection;
using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence.Analytics;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal sealed class CachedPortfolioAdvisorPerformanceRepository(
    PortfolioAdvisorPerformanceRepository requestRepository,
    IServiceScopeFactory scopeFactory,
    IPortfolioPerformanceCache cache,
    PortfolioControlCenterPerformanceOptions performance)
    : IPortfolioAdvisorPerformanceRepository
{
    private readonly TimeSpan _cacheDuration =
        TimeSpan.FromSeconds(performance.DetailCacheSeconds);

    public Task<IReadOnlyList<PortfolioAdvisorPerformanceDbRow>?> GetAdvisorPerformanceAsync(
        int crmClientId,
        PortfolioAdvisorPerformanceRequest request,
        CancellationToken cancellationToken)
    {
        if (_cacheDuration <= TimeSpan.Zero)
        {
            return requestRepository.GetAdvisorPerformanceAsync(
                crmClientId,
                request,
                cancellationToken);
        }

        return cache.GetOrCreateAsync(
            BuildCacheKey(crmClientId, request),
            _cacheDuration,
            token => ExecuteCacheMissAsync(
                crmClientId,
                request,
                token),
            cancellationToken);
    }

    private async Task<IReadOnlyList<PortfolioAdvisorPerformanceDbRow>?> ExecuteCacheMissAsync(
        int crmClientId,
        PortfolioAdvisorPerformanceRequest request,
        CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var repository = scope.ServiceProvider
            .GetRequiredService<PortfolioAdvisorPerformanceRepository>();

        return await repository.GetAdvisorPerformanceAsync(
            crmClientId,
            request,
            cancellationToken);
    }

    internal static string BuildCacheKey(
        int crmClientId,
        PortfolioAdvisorPerformanceRequest request) =>
        string.Create(
            System.Globalization.CultureInfo.InvariantCulture,
            $"pcc:advisor:v2:{crmClientId}:{request.BusinessUnit ?? "*"}:{request.Campaign ?? "*"}:{request.SubPortfolioId?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "*"}:{request.SupervisorId?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "*"}:{request.DateFrom?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture) ?? "*"}:{request.DateTo?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture) ?? "*"}");
}
