using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Persistence
{
    public class AnalyticsDbContext : DbContext
    {
        public DbSet<AnalyticsOptionConfig> AnalyticsOptionConfigs { get; set; }
        public DbSet<AnalyticsOptionClientScopeEntry> AnalyticsOptionClientScopes { get; set; }
        public DbSet<AnalyticsOptionGroupScopeEntry> AnalyticsOptionGroupScopes { get; set; }
        public DbSet<AnalyticsUserOptionScopeEntry> AnalyticsUserOptionScopes { get; set; }
        public DbSet<AnalyticsClientDimension> AnalyticsClients { get; set; }
        public DbSet<AnalyticsCampaignDimension> AnalyticsCampaigns { get; set; }
        public DbSet<AnalyticsCampaignDailySummary> AnalyticsCampaignDailySummaries { get; set; }
        public DbSet<AnalyticsCampaignEvolutionDaily> AnalyticsCampaignEvolutionDaily { get; set; }
        public DbSet<AnalyticsPortfolioEvolutionDaily> AnalyticsPortfolioEvolutionDaily { get; set; }
        public DbSet<AnalyticsPortfolioDailyFact> AnalyticsPortfolioDailyFacts { get; set; }
        public DbSet<AnalyticsPortfolioEvolutionDailyFact> AnalyticsPortfolioEvolutionDailyFacts { get; set; }
        public DbSet<AnalyticsPortfolioDimension> AnalyticsPortfolios { get; set; }
        public DbSet<AnalyticsDateDimension> AnalyticsDates { get; set; }
        public DbSet<AnalyticsSupervisorAdvisorDailyAttribution> AnalyticsSupervisorAdvisorDailyAttributions { get; set; }
        public DbSet<AnalyticsSupervisorDebtorContactDaily> AnalyticsSupervisorDebtorContactDaily { get; set; }
        public DbSet<AnalyticsSupervisorPromiseOperational> AnalyticsSupervisorPromiseOperational { get; set; }
        public DbSet<AnalyticsSupervisorDebtorPaymentDaily> AnalyticsSupervisorDebtorPaymentDaily { get; set; }
        public DbSet<AnalyticsAdvisorSupervisorCurrent> AnalyticsAdvisorSupervisorCurrent { get; set; }
        public DbSet<AnalyticsPortfolioSummaryStateDaily> AnalyticsPortfolioSummaryStateDaily { get; set; }
        public DbSet<AnalyticsPortfolioDailyMetric> AnalyticsPortfolioDailyMetrics { get; set; }
        public DbSet<AnalyticsTargetMonthlyFact> AnalyticsTargetMonthlyFacts { get; set; }
        public DbSet<AnalyticsDebtorContactDailyFact> AnalyticsDebtorContactDailyFacts { get; set; }
        public DbSet<AnalyticsPromiseFact> AnalyticsPromises { get; set; }
        public DbSet<AnalyticsDebtorPaymentDailyFact> AnalyticsDebtorPaymentDailyFacts { get; set; }
        public DbSet<AnalyticsWatermark> AnalyticsWatermarks { get; set; }
        public DbSet<AnalyticsCampaignTargetProgress> AnalyticsCampaignTargetProgress { get; set; }
        public DbSet<AnalyticsPromiseOperational> AnalyticsPromiseOperational { get; set; }
        public DbSet<AnalyticsReportClientScopeEntry> AnalyticsReportClientScopes { get; set; }
        public DbSet<AnalyticsReportClientEmbedEntry> AnalyticsReportClientEmbeds { get; set; }
        public DbSet<AnalyticsReportClientCatalogEntry> AnalyticsReportClientCatalog { get; set; }

        public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnalyticsOptionConfig>(builder =>
            {
                builder.ToTable("option_config", "analytics_access");
                builder.HasKey(option => option.OptionId);
                builder.Property(option => option.OptionId)
                    .HasColumnName("option_id")
                    .ValueGeneratedNever();
                builder.Property(option => option.OptionCode).HasColumnName("option_code");
                builder.Property(option => option.OptionName).HasColumnName("option_name");
                builder.Property(option => option.IsActive).HasColumnName("is_active");
                builder.Property(option => option.CreatedBy).HasColumnName("created_by");
                builder.Property(option => option.CreatedAt).HasColumnName("created_at");
                builder.Property(option => option.UpdatedBy).HasColumnName("updated_by");
                builder.Property(option => option.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<AnalyticsOptionClientScopeEntry>(builder =>
            {
                builder.ToTable("option_client_scope", "analytics_access");
                builder.HasKey(scope => new { scope.OptionId, scope.CrmClientId });
                builder.Property(scope => scope.OptionId).HasColumnName("option_id");
                builder.Property(scope => scope.CrmClientId).HasColumnName("crm_client_id");
                builder.Property(scope => scope.IsActive).HasColumnName("is_active");
                builder.Property(scope => scope.CreatedBy).HasColumnName("created_by");
                builder.Property(scope => scope.CreatedAt).HasColumnName("created_at");
                builder.Property(scope => scope.UpdatedBy).HasColumnName("updated_by");
                builder.Property(scope => scope.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<AnalyticsOptionGroupScopeEntry>(builder =>
            {
                builder.ToTable("option_group_scope", "analytics_access");
                builder.HasKey(scope => new { scope.OptionId, scope.SisgesGroupId });
                builder.Property(scope => scope.OptionId).HasColumnName("option_id");
                builder.Property(scope => scope.SisgesGroupId).HasColumnName("sisges_group_id");
                builder.Property(scope => scope.IsActive).HasColumnName("is_active");
                builder.Property(scope => scope.CreatedBy).HasColumnName("created_by");
                builder.Property(scope => scope.CreatedAt).HasColumnName("created_at");
                builder.Property(scope => scope.UpdatedBy).HasColumnName("updated_by");
                builder.Property(scope => scope.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<AnalyticsUserOptionScopeEntry>(builder =>
            {
                builder.ToTable("user_option_scope", "analytics_access");
                builder.HasKey(scope => new { scope.UserId, scope.OptionId });
                builder.Property(scope => scope.UserId).HasColumnName("user_id");
                builder.Property(scope => scope.OptionId).HasColumnName("option_id");
                builder.Property(scope => scope.IsActive).HasColumnName("is_active");
                builder.Property(scope => scope.CreatedBy).HasColumnName("created_by");
                builder.Property(scope => scope.CreatedAt).HasColumnName("created_at");
                builder.Property(scope => scope.UpdatedBy).HasColumnName("updated_by");
                builder.Property(scope => scope.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<AnalyticsClientDimension>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("dim_client", "analytics");
                builder.Property(client => client.ClientKey).HasColumnName("client_key");
                builder.Property(client => client.CrmClientId).HasColumnName("crm_client_id");
                builder.Property(client => client.ClientCode).HasColumnName("client_code");
                builder.Property(client => client.ClientName).HasColumnName("client_name");
            });

            modelBuilder.Entity<AnalyticsCampaignDimension>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("dim_campaign", "analytics");
                builder.Property(campaign => campaign.CampaignKey).HasColumnName("campaign_key");
                builder.Property(campaign => campaign.ClientKey).HasColumnName("client_key");
                builder.Property(campaign => campaign.CampaignCode).HasColumnName("campaign_code");
                builder.Property(campaign => campaign.CampaignName).HasColumnName("campaign_name");
                builder.Property(campaign => campaign.StartDate).HasColumnName("start_date");
                builder.Property(campaign => campaign.EndDate).HasColumnName("end_date");
            });

            modelBuilder.Entity<AnalyticsCampaignDailySummary>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("v_campaign_daily_summary", "analytics");
                builder.Property(row => row.ClientKey).HasColumnName("client_key");
                builder.Property(row => row.CampaignKey).HasColumnName("campaign_key");
                builder.Property(row => row.CalendarDate).HasColumnName("calendar_date");
                builder.Property(row => row.LoadedAt).HasColumnName("loaded_at");
            });

            modelBuilder.Entity<AnalyticsCampaignEvolutionDaily>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("v_campaign_evolution_daily", "analytics");
                builder.Property(row => row.ClientKey).HasColumnName("client_key");
                builder.Property(row => row.CampaignKey).HasColumnName("campaign_key");
                builder.Property(row => row.CalendarDate).HasColumnName("calendar_date");
                builder.Property(row => row.AssignedClients).HasColumnName("assigned_clients");
                builder.Property(row => row.ManagedClients).HasColumnName("managed_clients");
                builder.Property(row => row.PendingClients).HasColumnName("pending_clients");
                builder.Property(row => row.RecoveredAmountToDate).HasColumnName("recovered_amount_to_date");
                builder.Property(row => row.LoadedAt).HasColumnName("loaded_at");
            });

            modelBuilder.Entity<AnalyticsPortfolioEvolutionDaily>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("v_portfolio_evolution_daily", "analytics");
                builder.Property(row => row.ClientKey).HasColumnName("client_key");
                builder.Property(row => row.CampaignKey).HasColumnName("campaign_key");
                builder.Property(row => row.PortfolioKey).HasColumnName("portfolio_key");
                builder.Property(row => row.CalendarDate).HasColumnName("calendar_date");
                builder.Property(row => row.AssignedClients).HasColumnName("assigned_clients");
                builder.Property(row => row.ManagedClients).HasColumnName("managed_clients");
                builder.Property(row => row.PendingClients).HasColumnName("pending_clients");
                builder.Property(row => row.RecoveredAmountToDate).HasColumnName("recovered_amount_to_date");
                builder.Property(row => row.LoadedAt).HasColumnName("loaded_at");
            });

            modelBuilder.Entity<AnalyticsPortfolioDailyFact>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("fact_portfolio_daily", "analytics");
                builder.Property(row => row.ClientKey).HasColumnName("client_key");
                builder.Property(row => row.CampaignKey).HasColumnName("campaign_key");
                builder.Property(row => row.PortfolioKey).HasColumnName("portfolio_key");
                builder.Property(row => row.DateKey).HasColumnName("date_key");
                builder.Property(row => row.AssignedClientsSnapshot).HasColumnName("assigned_clients_snapshot");
                builder.Property(row => row.ManagedClientsSnapshot).HasColumnName("managed_clients_snapshot");
                builder.Property(row => row.PendingClientsSnapshot).HasColumnName("pending_clients_snapshot");
                builder.Property(row => row.ContactedClientsSnapshot).HasColumnName("contacted_clients_snapshot");
                builder.Property(row => row.HasSourceSnapshot).HasColumnName("has_source_snapshot");
                builder.Property(row => row.LoadedAt).HasColumnName("loaded_at");
            });

            modelBuilder.Entity<AnalyticsPortfolioEvolutionDailyFact>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("fact_portfolio_evolution_daily", "analytics");
                builder.Property(row => row.ClientKey).HasColumnName("client_key");
                builder.Property(row => row.CampaignKey).HasColumnName("campaign_key");
                builder.Property(row => row.PortfolioKey).HasColumnName("portfolio_key");
                builder.Property(row => row.DateKey).HasColumnName("date_key");
                builder.Property(row => row.AssignedClients).HasColumnName("assigned_clients");
                builder.Property(row => row.ManagedClients).HasColumnName("managed_clients");
                builder.Property(row => row.PendingClients).HasColumnName("pending_clients");
                builder.Property(row => row.LoadedAt).HasColumnName("loaded_at");
            });

            modelBuilder.Entity<AnalyticsPortfolioDimension>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("dim_portfolio", "analytics");
                builder.Property(portfolio => portfolio.PortfolioKey).HasColumnName("portfolio_key");
                builder.Property(portfolio => portfolio.PortfolioName).HasColumnName("portfolio_name");
                builder.Property(portfolio => portfolio.SourceBusinessUnit).HasColumnName("source_business_unit");
            });

            modelBuilder.Entity<AnalyticsDateDimension>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("dim_date", "analytics");
                builder.Property(date => date.DateKey).HasColumnName("date_key");
                builder.Property(date => date.CalendarDate).HasColumnName("calendar_date");
            });

            modelBuilder.Entity<AnalyticsSupervisorAdvisorDailyAttribution>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("v_supervisor_advisor_daily_attribution", "analytics");
                builder.Property(row => row.ClientKey).HasColumnName("client_key");
                builder.Property(row => row.CampaignKey).HasColumnName("campaign_key");
                builder.Property(row => row.PortfolioKey).HasColumnName("portfolio_key");
                builder.Property(row => row.AdvisorKey).HasColumnName("advisor_key");
                builder.Property(row => row.AdvisorName).HasColumnName("advisor_name");
                builder.Property(row => row.SupervisorKey).HasColumnName("supervisor_key");
                builder.Property(row => row.SupervisorName).HasColumnName("supervisor_name");
                builder.Property(row => row.CalendarDate).HasColumnName("calendar_date");
                builder.Property(row => row.ManagementEvents).HasColumnName("management_events");
                builder.Property(row => row.RecoveredAmount).HasColumnName("recovered_amount");
                builder.Property(row => row.LoadedAt).HasColumnName("loaded_at");
            });

            modelBuilder.Entity<AnalyticsSupervisorDebtorContactDaily>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("v_supervisor_debtor_contact_daily", "analytics");
                builder.Property(row => row.ClientKey).HasColumnName("client_key");
                builder.Property(row => row.CampaignKey).HasColumnName("campaign_key");
                builder.Property(row => row.PortfolioKey).HasColumnName("portfolio_key");
                builder.Property(row => row.SourceDebtorId).HasColumnName("source_debtor_id");
                builder.Property(row => row.AdvisorKey).HasColumnName("advisor_key");
                builder.Property(row => row.SupervisorKey).HasColumnName("supervisor_key");
                builder.Property(row => row.CalendarDate).HasColumnName("calendar_date");
                builder.Property(row => row.HadDirectContact).HasColumnName("had_direct_contact");
                builder.Property(row => row.HadIndirectContact).HasColumnName("had_indirect_contact");
                builder.Property(row => row.HadNoContact).HasColumnName("had_no_contact");
                builder.Property(row => row.LoadedAt).HasColumnName("loaded_at");
            });

            modelBuilder.Entity<AnalyticsSupervisorPromiseOperational>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("v_supervisor_promise_operational", "analytics");
                builder.Property(row => row.PromiseFactKey).HasColumnName("promise_fact_key");
                builder.Property(row => row.SupervisorAdvisorKey).HasColumnName("supervisor_advisor_key");
                builder.Property(row => row.ClientKey).HasColumnName("client_key");
                builder.Property(row => row.CampaignKey).HasColumnName("campaign_key");
                builder.Property(row => row.PortfolioKey).HasColumnName("portfolio_key");
                builder.Property(row => row.SourceDebtorId).HasColumnName("source_debtor_id");
                builder.Property(row => row.PromiseDueDate).HasColumnName("promise_due_date");
                builder.Property(row => row.AdvisorKey).HasColumnName("advisor_key");
                builder.Property(row => row.AdvisorName).HasColumnName("advisor_name");
                builder.Property(row => row.SupervisorKey).HasColumnName("supervisor_key");
                builder.Property(row => row.SupervisorName).HasColumnName("supervisor_name");
                builder.Property(row => row.IsValidPromise).HasColumnName("is_valid_promise");
                builder.Property(row => row.ManagementAt).HasColumnName("management_at");
                builder.Property(row => row.StatusCode).HasColumnName("status_code");
                builder.Property(row => row.PaidAmount).HasColumnName("paid_amount");
                builder.Property(row => row.PromiseAmount).HasColumnName("promise_amount");
                builder.Property(row => row.LoadedAt).HasColumnName("loaded_at");
            });

            modelBuilder.Entity<AnalyticsSupervisorDebtorPaymentDaily>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("v_supervisor_debtor_payment_daily", "analytics");
                builder.Property(row => row.ClientKey).HasColumnName("client_key");
                builder.Property(row => row.CampaignKey).HasColumnName("campaign_key");
                builder.Property(row => row.PortfolioKey).HasColumnName("portfolio_key");
                builder.Property(row => row.SourceDebtorId).HasColumnName("source_debtor_id");
                builder.Property(row => row.AdvisorKey).HasColumnName("advisor_key");
                builder.Property(row => row.SupervisorKey).HasColumnName("supervisor_key");
                builder.Property(row => row.CalendarDate).HasColumnName("calendar_date");
                builder.Property(row => row.LoadedAt).HasColumnName("loaded_at");
            });

            modelBuilder.Entity<AnalyticsAdvisorSupervisorCurrent>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("v_advisor_supervisor_current", "analytics");
                builder.Property(row => row.ClientKey).HasColumnName("client_key");
                builder.Property(row => row.AdvisorKey).HasColumnName("advisor_key");
                builder.Property(row => row.SupervisorKey).HasColumnName("supervisor_key");
                builder.Property(row => row.SupervisorName).HasColumnName("supervisor_name");
            });

            modelBuilder.Entity<AnalyticsPortfolioSummaryStateDaily>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("v_portfolio_summary_state_daily", "analytics");
                builder.Property(row => row.ClientKey).HasColumnName("client_key");
                builder.Property(row => row.CampaignKey).HasColumnName("campaign_key");
                builder.Property(row => row.PortfolioKey).HasColumnName("portfolio_key");
                builder.Property(row => row.CalendarDate).HasColumnName("calendar_date");
            });

            modelBuilder.Entity<AnalyticsPortfolioDailyMetric>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("v_portfolio_daily_metrics", "analytics");
                builder.Property(row => row.ClientKey).HasColumnName("client_key");
                builder.Property(row => row.CampaignKey).HasColumnName("campaign_key");
                builder.Property(row => row.PortfolioKey).HasColumnName("portfolio_key");
                builder.Property(row => row.CalendarDate).HasColumnName("calendar_date");
                builder.Property(row => row.HasSourceSnapshot).HasColumnName("has_source_snapshot");
                builder.Property(row => row.AssignedClientsSnapshot).HasColumnName("assigned_clients_snapshot");
                builder.Property(row => row.ManagedClientsSnapshot).HasColumnName("managed_clients_snapshot");
                builder.Property(row => row.PendingClientsSnapshot).HasColumnName("pending_clients_snapshot");
                builder.Property(row => row.ContactedClientsSnapshot).HasColumnName("contacted_clients_snapshot");
                builder.Property(row => row.ManagementEventsDay).HasColumnName("management_events_day");
                builder.Property(row => row.RecoveredAmountDay).HasColumnName("recovered_amount_day");
                builder.Property(row => row.LoadedAt).HasColumnName("loaded_at");
            });

            modelBuilder.Entity<AnalyticsTargetMonthlyFact>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("fact_target_monthly", "analytics");
                builder.Property(row => row.ClientKey).HasColumnName("client_key");
                builder.Property(row => row.CampaignKey).HasColumnName("campaign_key");
                builder.Property(row => row.PortfolioKey).HasColumnName("portfolio_key");
                builder.Property(row => row.TargetRecoveredAmount).HasColumnName("target_recovered_amount");
                builder.Property(row => row.SourceAsOfAt).HasColumnName("source_as_of_at");
            });

            modelBuilder.Entity<AnalyticsDebtorContactDailyFact>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("fact_debtor_contact_daily", "analytics");
                builder.Property(row => row.ClientKey).HasColumnName("client_key");
                builder.Property(row => row.CampaignKey).HasColumnName("campaign_key");
                builder.Property(row => row.PortfolioKey).HasColumnName("portfolio_key");
                builder.Property(row => row.SourceDebtorId).HasColumnName("source_debtor_id");
                builder.Property(row => row.DateKey).HasColumnName("date_key");
                builder.Property(row => row.HadDirectContact).HasColumnName("had_direct_contact");
                builder.Property(row => row.HadIndirectContact).HasColumnName("had_indirect_contact");
                builder.Property(row => row.HadNoContact).HasColumnName("had_no_contact");
                builder.Property(row => row.LoadedAt).HasColumnName("loaded_at");
            });

            modelBuilder.Entity<AnalyticsPromiseFact>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("fact_promise", "analytics");
                builder.Property(row => row.ClientKey).HasColumnName("client_key");
                builder.Property(row => row.CampaignKey).HasColumnName("campaign_key");
                builder.Property(row => row.PortfolioKey).HasColumnName("portfolio_key");
                builder.Property(row => row.SourceDebtorId).HasColumnName("source_debtor_id");
                builder.Property(row => row.IsValidPromise).HasColumnName("is_valid_promise");
                builder.Property(row => row.ManagementAt).HasColumnName("management_at");
                builder.Property(row => row.StatusCode).HasColumnName("status_code");
                builder.Property(row => row.PaidAmount).HasColumnName("paid_amount");
                builder.Property(row => row.PromiseAmount).HasColumnName("promise_amount");
                builder.Property(row => row.LoadedAt).HasColumnName("loaded_at");
            });

            modelBuilder.Entity<AnalyticsDebtorPaymentDailyFact>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("fact_debtor_payment_daily", "analytics");
                builder.Property(row => row.ClientKey).HasColumnName("client_key");
                builder.Property(row => row.CampaignKey).HasColumnName("campaign_key");
                builder.Property(row => row.PortfolioKey).HasColumnName("portfolio_key");
                builder.Property(row => row.SourceDebtorId).HasColumnName("source_debtor_id");
                builder.Property(row => row.DateKey).HasColumnName("date_key");
                builder.Property(row => row.LoadedAt).HasColumnName("loaded_at");
            });

            modelBuilder.Entity<AnalyticsWatermark>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("watermark", "etl");
                builder.Property(row => row.SourceCode).HasColumnName("source_code");
                builder.Property(row => row.LastSourceDateTime).HasColumnName("last_source_datetime");
                builder.Property(row => row.LastSuccessAt).HasColumnName("last_success_at");
            });

            modelBuilder.Entity<AnalyticsCampaignTargetProgress>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("v_campaign_target_progress", "analytics");
                builder.Property(row => row.ClientKey).HasColumnName("client_key");
                builder.Property(row => row.CampaignKey).HasColumnName("campaign_key");
                builder.Property(row => row.CalendarDate).HasColumnName("calendar_date");
                builder.Property(row => row.TargetRecoveredAmount).HasColumnName("target_recovered_amount");
                builder.Property(row => row.ExpectedRecoveredToDate).HasColumnName("expected_recovered_to_date");
                builder.Property(row => row.TargetSourceAsOfAt).HasColumnName("target_source_as_of_at");
            });

            modelBuilder.Entity<AnalyticsPromiseOperational>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("v_promise_operational", "analytics");
                builder.Property(row => row.PromiseFactKey).HasColumnName("promise_fact_key");
                builder.Property(row => row.ClientKey).HasColumnName("client_key");
                builder.Property(row => row.CampaignKey).HasColumnName("campaign_key");
                builder.Property(row => row.PortfolioKey).HasColumnName("portfolio_key");
                builder.Property(row => row.SourceDebtorId).HasColumnName("source_debtor_id");
                builder.Property(row => row.PromiseDueDate).HasColumnName("promise_due_date");
                builder.Property(row => row.LastPaymentDate).HasColumnName("last_payment_date");
                builder.Property(row => row.AdvisorKey).HasColumnName("advisor_key");
                builder.Property(row => row.IsValidPromise).HasColumnName("is_valid_promise");
                builder.Property(row => row.IsDueToday).HasColumnName("is_due_today");
                builder.Property(row => row.IsBroken).HasColumnName("is_broken");
                builder.Property(row => row.IsFulfilledOrPartial).HasColumnName("is_fulfilled_or_partial");
                builder.Property(row => row.PromiseAmount).HasColumnName("promise_amount");
                builder.Property(row => row.PaidAmount).HasColumnName("paid_amount");
                builder.Property(row => row.LoadedAt).HasColumnName("loaded_at");
            });

            modelBuilder.Entity<AnalyticsReportClientScopeEntry>(builder =>
            {
                builder.ToTable("power_bi_report_client_scope", "analytics_access");
                builder.HasKey(scope => new
                {
                    scope.OptionId,
                    scope.CrmClientId,
                    scope.ReportClientValue,
                    scope.SisgesGroupId
                });
                builder.Property(scope => scope.OptionId).HasColumnName("option_id");
                builder.Property(scope => scope.CrmClientId).HasColumnName("crm_client_id");
                builder.Property(scope => scope.ReportClientValue)
                    .HasColumnName("report_client_value")
                    .HasMaxLength(150)
                    .IsUnicode(false);
                builder.Property(scope => scope.SisgesGroupId).HasColumnName("sisges_group_id");
                builder.Property(scope => scope.IsActive).HasColumnName("is_active");
                builder.Property(scope => scope.CreatedBy).HasColumnName("created_by");
                builder.Property(scope => scope.CreatedAt).HasColumnName("created_at");
                builder.Property(scope => scope.UpdatedBy).HasColumnName("updated_by");
                builder.Property(scope => scope.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<AnalyticsReportClientEmbedEntry>(builder =>
            {
                builder.ToTable("power_bi_report_client_embed", "analytics_access");
                builder.HasKey(embed => new
                {
                    embed.OptionId,
                    embed.CrmClientId,
                    embed.ReportClientValue
                });
                builder.Property(embed => embed.OptionId).HasColumnName("option_id");
                builder.Property(embed => embed.CrmClientId).HasColumnName("crm_client_id");
                builder.Property(embed => embed.ReportClientValue)
                    .HasColumnName("report_client_value")
                    .HasMaxLength(150)
                    .IsUnicode(false);
                builder.Property(embed => embed.EmbedUrl)
                    .HasColumnName("embed_url")
                    .HasMaxLength(2048)
                    .IsUnicode(false);
                builder.Property(embed => embed.IsActive).HasColumnName("is_active");
                builder.Property(embed => embed.CreatedBy).HasColumnName("created_by");
                builder.Property(embed => embed.CreatedAt).HasColumnName("created_at");
                builder.Property(embed => embed.UpdatedBy).HasColumnName("updated_by");
                builder.Property(embed => embed.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<AnalyticsReportClientCatalogEntry>(builder =>
            {
                builder.HasNoKey();
                builder.ToTable("power_bi_report_client_catalog", "analytics_access");
                builder.Property(catalog => catalog.OptionId).HasColumnName("option_id");
                builder.Property(catalog => catalog.CrmClientId).HasColumnName("crm_client_id");
                builder.Property(catalog => catalog.ReportClientValue)
                    .HasColumnName("report_client_value")
                    .HasMaxLength(150)
                    .IsUnicode(false);
            });
        }
    }
}
