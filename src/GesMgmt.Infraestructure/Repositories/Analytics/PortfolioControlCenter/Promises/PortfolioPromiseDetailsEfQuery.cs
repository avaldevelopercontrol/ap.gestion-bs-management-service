using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal static class PortfolioPromiseDetailsEfQuery
{
    public static IQueryable<AnalyticsPromiseOperational> ApplyScope(
        AnalyticsDbContext context,
        IQueryable<AnalyticsPromiseOperational> query,
        long? subPortfolioId,
        string? businessUnit)
    {
        if (subPortfolioId.HasValue)
        {
            query = query.Where(row => row.PortfolioKey == subPortfolioId.Value);
        }

        if (businessUnit is null)
        {
            return query;
        }

        var portfolioKeys = context.AnalyticsPortfolios
            .AsNoTracking()
            .Where(portfolio => portfolio.SourceBusinessUnit == businessUnit)
            .Select(portfolio => portfolio.PortfolioKey);

        return query.Where(row => portfolioKeys.Contains(row.PortfolioKey));
    }

    public static IQueryable<AnalyticsSupervisorPromiseOperational> ApplyScope(
        AnalyticsDbContext context,
        IQueryable<AnalyticsSupervisorPromiseOperational> query,
        long? subPortfolioId,
        string? businessUnit)
    {
        if (subPortfolioId.HasValue)
        {
            query = query.Where(row => row.PortfolioKey == subPortfolioId.Value);
        }

        if (businessUnit is null)
        {
            return query;
        }

        var portfolioKeys = context.AnalyticsPortfolios
            .AsNoTracking()
            .Where(portfolio => portfolio.SourceBusinessUnit == businessUnit)
            .Select(portfolio => portfolio.PortfolioKey);

        return query.Where(row => portfolioKeys.Contains(row.PortfolioKey));
    }

    public static decimal RoundAmount(decimal value) =>
        decimal.Round(value, 4, MidpointRounding.AwayFromZero);

    public static string? NormalizeName(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    public static int GetOffset(int page, int pageSize) =>
        checked((page - 1) * pageSize);
}
