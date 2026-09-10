using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

public sealed record PortfolioPromisesCampaign(
    string Code,
    string Name);
public sealed record PortfolioPromiseStatusMetrics(
    long DueTodayCount,
    decimal DueTodayAmount,
    long OverdueCount,
    decimal? FulfillmentRate);
public sealed record PortfolioPromisesResponse(
    PortfolioPromisesCampaign Campaign,
    DateTimeOffset? UpdatedAt,
    PortfolioPromiseStatusMetrics Promises);
