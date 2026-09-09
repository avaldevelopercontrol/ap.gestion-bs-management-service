using GesMgmt.Application.Interfaces;
using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.Services.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Services.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Repositories.Analytics;
using GesMgmt.Application.Interfaces.Agenda;
using GesMgmt.Application.Interfaces.Cartera;
using GesMgmt.Application.Interfaces.Cliente;
using GesMgmt.Application.Interfaces.Deudor;
using GesMgmt.Application.Interfaces.Direccion;
using GesMgmt.Application.Interfaces.Email;
using GesMgmt.Application.Interfaces.Gestion;
using GesMgmt.Application.Interfaces.Grupo;
using GesMgmt.Application.Interfaces.Opcion;
using GesMgmt.Application.Interfaces.Perfil;
using GesMgmt.Application.Interfaces.PerfilOpcion;
using GesMgmt.Application.Interfaces.Telefono;
using GesMgmt.Application.Interfaces.UGrupo;
using GesMgmt.Application.Interfaces.Usuario;
using GesMgmt.Application.Interfaces.UsuarioGrupoOpcion;
using GesMgmt.Application.Services;
using GesMgmt.Application.Services.Agenda;
using GesMgmt.Application.Services.Cartera;
using GesMgmt.Application.Services.Cliente;
using GesMgmt.Application.Services.Deudor;
using GesMgmt.Application.Services.Direccion;
using GesMgmt.Application.Services.Email;
using GesMgmt.Application.Services.Gestion;
using GesMgmt.Application.Services.Grupo;
using GesMgmt.Application.Services.OpcionService;
using GesMgmt.Application.Services.Perfil;
using GesMgmt.Application.Services.PerfilOpcion;
using GesMgmt.Application.Services.Telefono;
using GesMgmt.Application.Services.UGrupo;
using GesMgmt.Application.Services.Usuario;
using GesMgmt.Application.Services.UsuarioGrupoOpcion;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Logger;
using GesMgmt.Infraestructure.Persistence;
using GesMgmt.Infraestructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GesMgmt.Infraestructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfraestructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuración de la cadena de conexión
            var connectionString = configuration.GetConnectionString("AvalCobConnection");
            var analyticsConnectionString = configuration.GetConnectionString("AvalAnalyticsConnection");
            var analyticsCommandTimeoutSeconds =
                configuration.GetValue<int?>("AnalyticsDatabase:CommandTimeoutSeconds") ?? 15;

            if (analyticsCommandTimeoutSeconds is <= 0 or > 120)
            {
                throw new InvalidOperationException(
                    "AnalyticsDatabase:CommandTimeoutSeconds debe estar entre 1 y 120.");
            }

            services.AddDbContext<AvalDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddDbContext<AnalyticsDbContext>(options =>
                options.UseSqlServer(
                    analyticsConnectionString,
                    sqlServerOptions =>
                        sqlServerOptions.CommandTimeout(analyticsCommandTimeoutSeconds)));
            AddAnalyticsAccess(services, configuration);
            AddPortfolioControlCenter(services, configuration);

            // Memoria
            services.AddMemoryCache();

            // Repositories
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<Iav_DocxCobrarRepository, av_DocxCobrarRepository>();
            services.AddScoped<IValidationMessageRepository, ValidationMessageRespository>();

            // Services
            services.AddScoped<IAgendaService, AgendaService>();
            services.AddScoped<ICarteraService, CarteraService>();
            services.AddScoped<IClienteService, ClienteService>();
            services.AddScoped<IDeudorService, DeudorService>();
            services.AddScoped<IDireccionService, DireccionService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IGestionService, GestionService>();
            services.AddScoped<IGrupoService, GrupoService>();
            services.AddScoped<IOpcionService, OpcionService>();
            services.AddScoped<IPerfilService, PerfilService>();
            services.AddScoped<IPerfilOpcionService, PerfilOpcionService>();
            services.AddScoped<ITelefonoService, TelefonoService>();
            services.AddScoped<IUGrupoService, UGrupoService>();
            services.AddScoped<IUsuarioGrupoOpcionService, UsuarioGrupoOpcionService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            
            services.AddScoped<IValidationMessageService, ValidationMessageService>();

            // Logger
            services.AddScoped(typeof(IAppLogger), typeof(LoggerAdapter));

            return services;
        }
        private static void AddAnalyticsAccess(
            IServiceCollection services,
            IConfiguration configuration)
        {
            var allowPublishToWeb = true;
            var rawAllowPublishToWeb =
                configuration[$"{AnalyticsPowerBiSecurityOptions.SectionName}:AllowPublishToWeb"];

            if (!string.IsNullOrWhiteSpace(rawAllowPublishToWeb) &&
                !bool.TryParse(rawAllowPublishToWeb, out allowPublishToWeb))
            {
                throw new InvalidOperationException(
                    $"{AnalyticsPowerBiSecurityOptions.SectionName}:AllowPublishToWeb debe ser true o false.");
            }

            services.AddSingleton(new AnalyticsPowerBiSecurityOptions
            {
                AllowPublishToWeb = allowPublishToWeb
            });

            services.AddSingleton<IAnalyticsAccessCache, AnalyticsAccessMemoryCache>();

            services.AddScoped<ISisgesUserClientRepository, SisgesUserClientRepository>();
            services.AddScoped<ISisgesUserGroupRepository, SisgesUserGroupRepository>();
            services.AddScoped<ISisgesClientGroupRepository, SisgesClientGroupRepository>();

            services.AddScoped<ISisgesOptionPermissionRepository, SisgesOptionPermissionRepository>();
            services.AddScoped<IAnalyticsOptionConfigRepository, AnalyticsOptionConfigRepository>();
            services.AddScoped<IAnalyticsOptionClientScopeRepository, AnalyticsOptionClientScopeRepository>();
            services.AddScoped<IAnalyticsOptionGroupScopeRepository, AnalyticsOptionGroupScopeRepository>();
            services.AddScoped<IAnalyticsUserOptionRepository, AnalyticsUserOptionRepository>();
            services.AddScoped<IAnalyticsReportClientScopeRepository, AnalyticsReportClientScopeRepository>();
            services.AddScoped<IAnalyticsReportClientCatalogRepository, AnalyticsReportClientCatalogRepository>();
            services.AddScoped<IAnalyticsReportClientEmbedRepository, AnalyticsReportClientEmbedRepository>();
            services.AddScoped<IAnalyticsReportClientPublicationWriter, AnalyticsReportClientPublicationWriter>();
            services.AddScoped<IAnalyticsPowerBiConfigurationWriter, AnalyticsPowerBiConfigurationWriter>();

            services.AddScoped<IAnalyticsAuthorizationService, AnalyticsAuthorizationService>();
            services.AddScoped<IAnalyticsOptionService, AnalyticsOptionService>();
            services.AddScoped<IAnalyticsUserOptionQueryService, AnalyticsUserOptionQueryService>();
            services.AddScoped<IAnalyticsAccessService, AnalyticsAccessService>();
            services.AddScoped<IAnalyticsGroupAccessService, AnalyticsGroupAccessService>();
            services.AddScoped<IAnalyticsOptionAccessService, AnalyticsOptionAccessService>();
            services.AddScoped<IAnalyticsOptionClientAdministrationService, AnalyticsOptionClientAdministrationService>();
            services.AddScoped<IAnalyticsOptionGroupAdministrationService, AnalyticsOptionGroupAdministrationService>();
            services.AddScoped<IAnalyticsUserOptionAdministrationService, AnalyticsUserOptionAdministrationService>();
            services.AddScoped<IAnalyticsReportClientConfigurationService, AnalyticsReportClientConfigurationService>();
            services.AddScoped<IAnalyticsReportClientAccessService, AnalyticsReportClientAccessService>();
            services.AddScoped<IAnalyticsReportClientEmbedLookupService, AnalyticsReportClientEmbedLookupService>();
            services.AddScoped<IAnalyticsOptionReportClientEmbedsAdministrationService, AnalyticsOptionReportClientEmbedsAdministrationService>();
            services.AddScoped<IAnalyticsPowerBiConfigurationService, AnalyticsPowerBiConfigurationService>();
            services.AddSingleton<IAnalyticsPowerBiSecurityPolicy, AnalyticsPowerBiSecurityPolicy>();
            services.AddScoped<IAnalyticsPowerBiUserAccessService, AnalyticsPowerBiUserAccessService>();
            services.AddScoped<IAnalyticsPowerBiViewerContextService, AnalyticsPowerBiViewerContextService>();
        }


        private static void AddPortfolioControlCenter(
            IServiceCollection services,
            IConfiguration configuration)
        {
            var performance = configuration
                .GetSection(PortfolioControlCenterPerformanceOptions.SectionName)
                .Get<PortfolioControlCenterPerformanceOptions>()
                ?? new PortfolioControlCenterPerformanceOptions();
            performance.Validate();

            services.AddSingleton(performance);
            services.AddSingleton<IPortfolioPerformanceCache, PortfolioPerformanceMemoryCache>();

            services.AddScoped<IPortfolioControlCenterAccessService, PortfolioControlCenterAccessService>();

            services.AddScoped<IPortfolioSummaryRepository, PortfolioSummaryRepository>();
            services.AddScoped<PortfolioAdvisorPerformanceRepository>();
            services.AddScoped<IPortfolioAdvisorPerformanceRepository, CachedPortfolioAdvisorPerformanceRepository>();
            services.AddScoped<PortfolioSupervisorPerformanceRepository>();
            services.AddScoped<IPortfolioSupervisorPerformanceRepository, CachedPortfolioSupervisorPerformanceRepository>();
            services.AddScoped<IPortfolioCampaignPerformanceRepository, PortfolioCampaignPerformanceRepository>();
            services.AddScoped<IPortfolioBootstrapRepository, PortfolioBootstrapRepository>();
            services.AddScoped<IPortfolioEvolutionRepository, PortfolioEvolutionRepository>();
            services.AddScoped<IPortfolioFilterOptionsRepository, PortfolioFilterOptionsRepository>();
            services.AddScoped<IPortfolioOverviewRepository, PortfolioOverviewRepository>();
            services.AddScoped<IPortfolioPromisesRepository, PortfolioPromisesRepository>();
            services.AddScoped<IPortfolioDueTodayPromisesRepository, PortfolioDueTodayPromisesRepository>();
            services.AddScoped<IPortfolioOverduePromisesRepository, PortfolioOverduePromisesRepository>();
            services.AddScoped<IPortfolioTargetProgressRepository, PortfolioTargetProgressRepository>();

            services.AddScoped<IPortfolioAdvisorPerformanceService, PortfolioAdvisorPerformanceService>();
            services.AddScoped<IPortfolioBootstrapService, PortfolioBootstrapService>();
            services.AddScoped<IPortfolioCampaignPerformanceService, PortfolioCampaignPerformanceService>();
            services.AddScoped<IPortfolioEvolutionService, PortfolioEvolutionService>();
            services.AddScoped<IPortfolioFilterOptionsService, PortfolioFilterOptionsService>();
            services.AddScoped<IPortfolioOverviewService, PortfolioOverviewService>();
            services.AddScoped<IPortfolioPromisesService, PortfolioPromisesService>();
            services.AddScoped<IPortfolioDueTodayPromisesService, PortfolioDueTodayPromisesService>();
            services.AddScoped<IPortfolioOverduePromisesService, PortfolioOverduePromisesService>();
            services.AddScoped<IPortfolioSummaryService, PortfolioSummaryService>();
            services.AddScoped<IPortfolioSupervisorPerformanceService, PortfolioSupervisorPerformanceService>();
            services.AddScoped<IPortfolioTargetProgressService, PortfolioTargetProgressService>();
        }
    }
}