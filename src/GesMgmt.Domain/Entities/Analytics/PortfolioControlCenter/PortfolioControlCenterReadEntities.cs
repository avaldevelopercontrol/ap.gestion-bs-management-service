namespace GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;

public sealed class AnalyticsCampaignDimension
{
    public int CampaignKey { get; set; }
    public int ClientKey { get; set; }
    public string CampaignCode { get; set; } = string.Empty;
    public string CampaignName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public sealed class AnalyticsCampaignDailySummary
{
    public int ClientKey { get; set; }
    public int CampaignKey { get; set; }
    public DateTime CalendarDate { get; set; }
    public DateTime? LoadedAt { get; set; }
}

public sealed class AnalyticsCampaignEvolutionDaily
{
    public int ClientKey { get; set; }
    public int CampaignKey { get; set; }
    public DateTime CalendarDate { get; set; }
    public int? AssignedClients { get; set; }
    public int? ManagedClients { get; set; }
    public int? PendingClients { get; set; }
    public decimal? RecoveredAmountToDate { get; set; }
    public DateTime? LoadedAt { get; set; }
}

public sealed class AnalyticsPortfolioEvolutionDaily
{
    public int ClientKey { get; set; }
    public int CampaignKey { get; set; }
    public long PortfolioKey { get; set; }
    public DateTime CalendarDate { get; set; }
    public int? AssignedClients { get; set; }
    public int? ManagedClients { get; set; }
    public int? PendingClients { get; set; }
    public decimal? RecoveredAmountToDate { get; set; }
    public DateTime? LoadedAt { get; set; }
}

public sealed class AnalyticsPortfolioDailyFact
{
    public int ClientKey { get; set; }
    public int CampaignKey { get; set; }
    public long PortfolioKey { get; set; }
    public int DateKey { get; set; }
    public int? AssignedClientsSnapshot { get; set; }
    public int? ManagedClientsSnapshot { get; set; }
    public int? PendingClientsSnapshot { get; set; }
    public int? ContactedClientsSnapshot { get; set; }
    public bool HasSourceSnapshot { get; set; }
    public DateTime? LoadedAt { get; set; }
}

public sealed class AnalyticsPortfolioEvolutionDailyFact
{
    public int ClientKey { get; set; }
    public int CampaignKey { get; set; }
    public long PortfolioKey { get; set; }
    public int DateKey { get; set; }
    public int? AssignedClients { get; set; }
    public int? ManagedClients { get; set; }
    public int? PendingClients { get; set; }
    public DateTime? LoadedAt { get; set; }
}

public sealed class AnalyticsPortfolioDimension
{
    public long PortfolioKey { get; set; }
    public string PortfolioName { get; set; } = string.Empty;
    public string? SourceBusinessUnit { get; set; }
}

public sealed class AnalyticsDateDimension
{
    public int DateKey { get; set; }
    public DateTime CalendarDate { get; set; }
}

public sealed class AnalyticsSupervisorAdvisorDailyAttribution
{
    public int ClientKey { get; set; }
    public int CampaignKey { get; set; }
    public long PortfolioKey { get; set; }
    public int AdvisorKey { get; set; }
    public string AdvisorName { get; set; } = string.Empty;
    public int? SupervisorKey { get; set; }
    public string? SupervisorName { get; set; }
    public DateTime CalendarDate { get; set; }
    public int? ManagementEvents { get; set; }
    public decimal? RecoveredAmount { get; set; }
    public DateTime? LoadedAt { get; set; }
}

public sealed class AnalyticsSupervisorDebtorContactDaily
{
    public int ClientKey { get; set; }
    public int CampaignKey { get; set; }
    public long PortfolioKey { get; set; }
    public long SourceDebtorId { get; set; }
    public int? AdvisorKey { get; set; }
    public int? SupervisorKey { get; set; }
    public DateTime CalendarDate { get; set; }
    public bool HadDirectContact { get; set; }
    public bool HadIndirectContact { get; set; }
    public bool HadNoContact { get; set; }
    public DateTime? LoadedAt { get; set; }
}

public sealed class AnalyticsSupervisorPromiseOperational
{
    public long PromiseFactKey { get; set; }
    public long SupervisorAdvisorKey { get; set; }
    public int ClientKey { get; set; }
    public int CampaignKey { get; set; }
    public long PortfolioKey { get; set; }
    public long SourceDebtorId { get; set; }
    public DateTime? PromiseDueDate { get; set; }
    public int? AdvisorKey { get; set; }
    public string? AdvisorName { get; set; }
    public int? SupervisorKey { get; set; }
    public string? SupervisorName { get; set; }
    public bool IsValidPromise { get; set; }
    public DateTime ManagementAt { get; set; }
    public string StatusCode { get; set; } = string.Empty;
    public decimal? PaidAmount { get; set; }
    public decimal? PromiseAmount { get; set; }
    public DateTime? LoadedAt { get; set; }
}

public sealed class AnalyticsSupervisorDebtorPaymentDaily
{
    public int ClientKey { get; set; }
    public int CampaignKey { get; set; }
    public long PortfolioKey { get; set; }
    public long SourceDebtorId { get; set; }
    public int? AdvisorKey { get; set; }
    public int? SupervisorKey { get; set; }
    public DateTime CalendarDate { get; set; }
    public DateTime? LoadedAt { get; set; }
}

public sealed class AnalyticsAdvisorSupervisorCurrent
{
    public int ClientKey { get; set; }
    public int AdvisorKey { get; set; }
    public int? SupervisorKey { get; set; }
    public string? SupervisorName { get; set; }
}

public sealed class AnalyticsPortfolioSummaryStateDaily
{
    public int ClientKey { get; set; }
    public int CampaignKey { get; set; }
    public long PortfolioKey { get; set; }
    public DateTime CalendarDate { get; set; }
}

public sealed class AnalyticsPortfolioDailyMetric
{
    public int ClientKey { get; set; }
    public int CampaignKey { get; set; }
    public long PortfolioKey { get; set; }
    public DateTime CalendarDate { get; set; }
    public bool HasSourceSnapshot { get; set; }
    public int? AssignedClientsSnapshot { get; set; }
    public int? ManagedClientsSnapshot { get; set; }
    public int? PendingClientsSnapshot { get; set; }
    public int? ContactedClientsSnapshot { get; set; }
    public int? ManagementEventsDay { get; set; }
    public decimal? RecoveredAmountDay { get; set; }
    public DateTime? LoadedAt { get; set; }
}

public sealed class AnalyticsDebtorContactDailyFact
{
    public int ClientKey { get; set; }
    public int CampaignKey { get; set; }
    public long PortfolioKey { get; set; }
    public long SourceDebtorId { get; set; }
    public int DateKey { get; set; }
    public bool HadDirectContact { get; set; }
    public bool HadIndirectContact { get; set; }
    public bool HadNoContact { get; set; }
    public DateTime? LoadedAt { get; set; }
}

public sealed class AnalyticsPromiseFact
{
    public int ClientKey { get; set; }
    public int CampaignKey { get; set; }
    public long PortfolioKey { get; set; }
    public long SourceDebtorId { get; set; }
    public bool IsValidPromise { get; set; }
    public DateTime ManagementAt { get; set; }
    public string StatusCode { get; set; } = string.Empty;
    public decimal? PaidAmount { get; set; }
    public decimal? PromiseAmount { get; set; }
    public DateTime? LoadedAt { get; set; }
}

public sealed class AnalyticsDebtorPaymentDailyFact
{
    public int ClientKey { get; set; }
    public int CampaignKey { get; set; }
    public long PortfolioKey { get; set; }
    public long SourceDebtorId { get; set; }
    public int DateKey { get; set; }
    public DateTime? LoadedAt { get; set; }
}

public sealed class AnalyticsWatermark
{
    public string SourceCode { get; set; } = string.Empty;
    public DateTime? LastSourceDateTime { get; set; }
    public DateTime? LastSuccessAt { get; set; }
}


public sealed class AnalyticsTargetMonthlyFact
{
    public int ClientKey { get; set; }
    public int CampaignKey { get; set; }
    public long? PortfolioKey { get; set; }
    public decimal? TargetRecoveredAmount { get; set; }
    public DateTime? SourceAsOfAt { get; set; }
}

public sealed class AnalyticsCampaignTargetProgress
{
    public int ClientKey { get; set; }
    public int CampaignKey { get; set; }
    public DateTime CalendarDate { get; set; }
    public decimal? TargetRecoveredAmount { get; set; }
    public decimal? ExpectedRecoveredToDate { get; set; }
    public DateTime? TargetSourceAsOfAt { get; set; }
}

public sealed class AnalyticsPromiseOperational
{
    public long PromiseFactKey { get; set; }
    public int ClientKey { get; set; }
    public int CampaignKey { get; set; }
    public long PortfolioKey { get; set; }
    public long SourceDebtorId { get; set; }
    public DateTime? PromiseDueDate { get; set; }
    public DateTime? LastPaymentDate { get; set; }
    public int? AdvisorKey { get; set; }
    public bool IsValidPromise { get; set; }
    public bool IsDueToday { get; set; }
    public bool IsBroken { get; set; }
    public bool IsFulfilledOrPartial { get; set; }
    public decimal? PromiseAmount { get; set; }
    public decimal? PaidAmount { get; set; }
    public DateTime? LoadedAt { get; set; }
}
