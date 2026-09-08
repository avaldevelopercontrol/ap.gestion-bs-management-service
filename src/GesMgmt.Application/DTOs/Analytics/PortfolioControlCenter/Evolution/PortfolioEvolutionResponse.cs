using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

public sealed record PortfolioEvolutionCampaign(
    string Code,
    string Name);
public sealed record PortfolioEvolutionPeriod(
    DateOnly DateFrom,
    DateOnly DateTo);
public sealed record PortfolioEvolutionPoint(
    DateOnly Period,
    long AssignedPortfolio,
    long ManagedPortfolio,
    long PendingPortfolio,
    decimal RecoveredAmount);
public sealed record PortfolioEvolutionResponse(
    PortfolioEvolutionCampaign Campaign,
    PortfolioEvolutionPeriod Period,
    DateTimeOffset? UpdatedAt,
    IReadOnlyList<PortfolioEvolutionPoint> Evolution);
