using Microsoft.Extensions.DependencyInjection;
using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence.Analytics;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal sealed class CachedPortfolioSupervisorPerformanceRepository(
    PortfolioSupervisorPerformanceRepository requestRepository,
    IServiceScopeFactory scopeFactory,
    IPortfolioPerformanceCache cache,
    PortfolioControlCenterPerformanceOptions performance)
    : IPortfolioSupervisorPerformanceRepository
{
    private readonly TimeSpan _cacheDuration =
        TimeSpan.FromSeconds(performance.DetailCacheSeconds);

    public Task<IReadOnlyList<PortfolioSupervisorPerformanceDbRow>?> GetSupervisorPerformanceAsync(
        int crmClientId,
        PortfolioSupervisorPerformanceRequest request,
        CancellationToken cancellationToken)
    {
        if (_cacheDuration <= TimeSpan.Zero)
        {
            return requestRepository.GetSupervisorPerformanceAsync(
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

    private async Task<IReadOnlyList<PortfolioSupervisorPerformanceDbRow>?> ExecuteCacheMissAsync(
        int crmClientId,
        PortfolioSupervisorPerformanceRequest request,
        CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var repository = scope.ServiceProvider
            .GetRequiredService<PortfolioSupervisorPerformanceRepository>();

        return await repository.GetSupervisorPerformanceAsync(
            crmClientId,
            request,
            cancellationToken);
    }

    internal static string BuildCacheKey(
        int crmClientId,
        PortfolioSupervisorPerformanceRequest request) =>
        string.Create(
            System.Globalization.CultureInfo.InvariantCulture,
            $"pcc:supervisor:v2:{crmClientId}:{request.BusinessUnit ?? "*"}:{request.Campaign ?? "*"}:{request.SubPortfolioId?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "*"}:{request.SupervisorId?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "*"}:{request.DateFrom?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture) ?? "*"}:{request.DateTo?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture) ?? "*"}");
}
