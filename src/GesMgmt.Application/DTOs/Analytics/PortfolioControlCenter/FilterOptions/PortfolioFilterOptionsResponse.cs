using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
namespace GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

public sealed record PortfolioFilterCampaignOption(
    string Code,
    string Name,
    DateOnly StartDate,
    DateOnly EndDate,
    DateOnly AvailableDateFrom,
    DateOnly AvailableDateTo);

/// <summary>
/// Cartera de negocio autorizada para la sesión Analytics.
/// El nombre visible se resuelve en el CRM, mientras que Analytics mantiene
/// el identificador de scope (crm_client_id) como fuente de autorización.
/// </summary>
public sealed record PortfolioFilterPortfolioScope(int Id);
public sealed record PortfolioFilterBusinessUnitOption(
    string Code,
    string Name);
public sealed record PortfolioFilterSubPortfolioOption(
    long Id,
    string Name);
public sealed record PortfolioFilterSupervisorOption(
    int Id,
    string Name);
public sealed record SubPortfolioCampaignAvailability(
    long SubPortfolioId,
    string CampaignCode,
    DateOnly AvailableDateFrom,
    DateOnly AvailableDateTo);
public sealed record PortfolioSupervisorContextAvailability(
    int SupervisorId,
    long SubPortfolioId,
    string CampaignCode,
    DateOnly AvailableDateFrom,
    DateOnly AvailableDateTo);
public sealed record PortfolioFilterAvailability(
    IReadOnlyList<SubPortfolioCampaignAvailability> SubPortfolioCampaigns,
    IReadOnlyList<PortfolioSupervisorContextAvailability> SupervisorContexts);
public sealed record PortfolioFilterOptionsResponse(
    DateOnly? AvailableDateFrom,
    DateOnly? AvailableDateTo,
    DateTimeOffset? UpdatedAt,
    PortfolioFilterPortfolioScope Portfolio,
    IReadOnlyList<PortfolioFilterBusinessUnitOption> BusinessUnits,
    string? SelectedBusinessUnit,
    IReadOnlyList<PortfolioFilterCampaignOption> Campaigns,
    IReadOnlyList<PortfolioFilterSubPortfolioOption> SubPortfolios,
    IReadOnlyList<PortfolioFilterSupervisorOption> Supervisors,
    PortfolioFilterAvailability Availability);
