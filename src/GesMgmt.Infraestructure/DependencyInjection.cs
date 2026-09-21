using GesMgmt.Application.Interfaces;
using GesMgmt.Application.DTOs.Analitica;
using GesMgmt.Application.DTOs.Analitica.CentroControlCartera;
using GesMgmt.Application.Interfaces.Analitica.CentroControlCartera;
using GesMgmt.Application.Services.Analitica.CentroControlCartera;
using GesMgmt.Domain.Interfaces.Analitica.CentroControlCartera;
using GesMgmt.Infraestructure.Repositories.Analitica.CentroControlCartera;
using GesMgmt.Application.Interfaces.Analitica;
using GesMgmt.Application.Services.Analitica;
using GesMgmt.Application.Interfaces.Analitica.SesionesPowerBi;
using GesMgmt.Application.Services.Analitica.SesionesPowerBi;
using GesMgmt.Domain.Interfaces.Analitica;
using GesMgmt.Infraestructure.Repositories.Analitica;
using GesMgmt.Domain.Interfaces.Analitica.SesionesPowerBi;
using GesMgmt.Infraestructure.Repositories.Analitica.SesionesPowerBi;
using GesMgmt.Application.Interfaces.Agenda;
using GesMgmt.Application.Interfaces.Boton;
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
using GesMgmt.Application.Services.Boton;
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
        public static IServiceCollection AddInfraestructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Configuración de la cadena de conexión
            var connectionString = configuration.GetConnectionString("AvalCobConnection");
            var cadenaConexionAnalitica = configuration.GetConnectionString("AvalAnalyticsConnection");

            var segundosTimeoutComandoAnalitica =
                configuration.GetValue<int?>("AnalyticsDatabase:CommandTimeoutSeconds") ?? 15;

            if (segundosTimeoutComandoAnalitica is <= 0 or > 120)
            {
                throw new InvalidOperationException(
                    "AnalyticsDatabase:CommandTimeoutSeconds debe estar entre 1 y 120.");
            }

            services.AddDbContext<AvalDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddDbContext<AnaliticaDbContext>(options =>
                options.UseSqlServer(
                    cadenaConexionAnalitica,
                    sqlServerOptions =>
                        sqlServerOptions.CommandTimeout(segundosTimeoutComandoAnalitica)));

            AgregarAccesoAnalitica(services, configuration);
            AgregarCentroControlCartera(services, configuration);

            // Memoria
            services.AddMemoryCache();

            // Repositories
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<Iav_DocxCobrarRepository, av_DocxCobrarRepository>();
            services.AddScoped<IValidationMessageRepository, ValidationMessageRespository>();

            // Services
            services.AddScoped<IAgendaService, AgendaService>();
            services.AddScoped<IBotonService, BotonService>();
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

        private static void AgregarAccesoAnalitica(
            IServiceCollection services,
            IConfiguration configuration)
        {
            var permitirPublicarEnWeb = true;
            var valorPermitirPublicarEnWeb =
                configuration[$"{SeguridadPowerBiAnaliticaOptions.NombreSeccion}:AllowPublishToWeb"];

            if (!string.IsNullOrWhiteSpace(valorPermitirPublicarEnWeb) &&
                !bool.TryParse(valorPermitirPublicarEnWeb, out permitirPublicarEnWeb))
            {
                throw new InvalidOperationException(
                    $"{SeguridadPowerBiAnaliticaOptions.NombreSeccion}:AllowPublishToWeb debe ser true o false.");
            }

            services.AddSingleton(new SeguridadPowerBiAnaliticaOptions
            {
                PermitirPublicarEnWeb = permitirPublicarEnWeb
            });

            services.AddSingleton<ICacheAccesoAnalitica, AccesoAnaliticaMemoryCache>();

            services.AddScoped<ISisgesClienteUsuarioRepository, SisgesClienteUsuarioRepository>();
            services.AddScoped<ISisgesGrupoUsuarioRepository, SisgesGrupoUsuarioRepository>();
            services.AddScoped<ISisgesGrupoClienteRepository, SisgesGrupoClienteRepository>();

            services.AddScoped<ISisgesOpcionPermisoRepository, SisgesOpcionPermisoRepository>();
            services.AddScoped<IAnaliticaOpcionConfiguracionRepository, AnaliticaOpcionConfiguracionRepository>();
            services.AddScoped<IAnaliticaAlcanceClienteOpcionRepository, AnaliticaAlcanceClienteOpcionRepository>();
            services.AddScoped<IAnaliticaAlcanceGrupoOpcionRepository, AnaliticaAlcanceGrupoOpcionRepository>();
            services.AddScoped<IAnaliticaOpcionUsuarioRepository, AnaliticaOpcionUsuarioRepository>();
            services.AddScoped<IAnaliticaAlcanceReporteClienteRepository, AnaliticaAlcanceReporteClienteRepository>();
            services.AddScoped<IAnaliticaCatalogoReporteClienteRepository, AnaliticaCatalogoReporteClienteRepository>();
            services.AddScoped<IAnaliticaIncrustacionReporteClienteRepository, AnaliticaIncrustacionReporteClienteRepository>();
            services.AddScoped<IAnaliticaPublicacionReporteClienteWriter, AnaliticaPublicacionReporteClienteWriter>();
            services.AddScoped<IAnaliticaConfiguracionPowerBiWriter, AnaliticaConfiguracionPowerBiWriter>();

            services.AddScoped<IAutorizacionAnaliticaService, AutorizacionAnaliticaService>();
            services.AddScoped<IOpcionAnaliticaService, OpcionAnaliticaService>();
            services.AddScoped<IConsultaOpcionesUsuarioAnaliticaService, ConsultaOpcionesUsuarioAnaliticaService>();
            services.AddScoped<IAccesoAnaliticaService, AccesoAnaliticaService>();
            services.AddScoped<IAccesoGrupoAnaliticaService, AccesoGrupoAnaliticaService>();
            services.AddScoped<IAccesoOpcionAnaliticaService, AccesoOpcionAnaliticaService>();
            services.AddScoped<IAdministracionClientesOpcionAnaliticaService, AdministracionClientesOpcionAnaliticaService>();
            services.AddScoped<IAdministracionGruposOpcionAnaliticaService, AdministracionGruposOpcionAnaliticaService>();
            services.AddScoped<IAdministracionOpcionesUsuarioAnaliticaService, AdministracionOpcionesUsuarioAnaliticaService>();
            services.AddScoped<IConfiguracionReporteClienteAnaliticaService, ConfiguracionReporteClienteAnaliticaService>();
            services.AddScoped<IAccesoReporteClienteAnaliticaService, AccesoReporteClienteAnaliticaService>();
            services.AddScoped<IConsultaIncrustacionReporteClienteAnaliticaService, ConsultaIncrustacionReporteClienteAnaliticaService>();
            services.AddScoped<IAdministracionIncrustacionesReporteClienteOpcionAnaliticaService, AdministracionIncrustacionesReporteClienteOpcionAnaliticaService>();
            services.AddScoped<IConfiguracionPowerBiAnaliticaService, ConfiguracionPowerBiAnaliticaService>();
            services.AddSingleton<ISeguridadPowerBiAnaliticaPolicy, SeguridadPowerBiAnaliticaPolicy>();
            services.AddScoped<IAccesoUsuarioPowerBiAnaliticaService, AccesoUsuarioPowerBiAnaliticaService>();
            services.AddScoped<IContextoVisorPowerBiAnaliticaService, ContextoVisorPowerBiAnaliticaService>();

            // Sesiones Power BI
            services.AddScoped<ICatalogoSesionPowerBiRepository, CatalogoSesionPowerBiRepository>();
            services.AddScoped<ISesionPowerBiAnaliticaRepository, SesionPowerBiAnaliticaRepository>();
            services.AddScoped<IConsultaSesionesPowerBiRepository, ConsultaSesionesPowerBiRepository>();
            services.AddScoped<ISesionPowerBiAnaliticaService, SesionPowerBiAnaliticaService>();
            services.AddScoped<IConsultaSesionesPowerBiService, ConsultaSesionesPowerBiService>();
        }

        private static void AgregarCentroControlCartera(
            IServiceCollection services,
            IConfiguration configuration)
        {
            var performance = configuration
                .GetSection(RendimientoCentroControlCarteraOptions.NombreSeccion)
                .Get<RendimientoCentroControlCarteraOptions>()
                ?? new RendimientoCentroControlCarteraOptions();

            performance.Validar();

            services.AddSingleton(performance);
            services.AddSingleton(TimeProvider.System);
            services.AddSingleton<ICacheRendimientoCartera, RendimientoCarteraMemoryCache>();

            services.AddScoped<IAccesoCentroControlCarteraService, AccesoCentroControlCarteraService>();

            services.AddScoped<IResumenCarteraRepository, ResumenCarteraRepository>();
            services.AddScoped<RendimientoAsesorCarteraRepository>();
            services.AddScoped<IRendimientoAsesorCarteraRepository, RendimientoAsesorCarteraCacheRepository>();
            services.AddScoped<RendimientoSupervisorCarteraRepository>();
            services.AddScoped<IRendimientoSupervisorCarteraRepository, RendimientoSupervisorCarteraCacheRepository>();
            services.AddScoped<IRendimientoCampanaCarteraRepository, RendimientoCampanaCarteraRepository>();
            services.AddScoped<IInicializacionCarteraRepository, InicializacionCarteraRepository>();
            services.AddScoped<IEvolucionCarteraRepository, EvolucionCarteraRepository>();
            services.AddScoped<IOpcionesFiltroCarteraRepository, OpcionesFiltroCarteraRepository>();
            services.AddScoped<IPanoramaCarteraRepository, PanoramaCarteraRepository>();
            services.AddScoped<IPromesasCarteraRepository, PromesasCarteraRepository>();
            services.AddScoped<IPromesasVencenHoyCarteraRepository, PromesasVencenHoyCarteraRepository>();
            services.AddScoped<ISeguimientoPromesasCarteraRepository, SeguimientoPromesasCarteraRepository>();
            services.AddScoped<IPromesasVencidasCarteraRepository, PromesasVencidasCarteraRepository>();
            services.AddScoped<IAvanceMetaCarteraRepository, AvanceMetaCarteraRepository>();

            services.AddScoped<IRendimientoAsesorCarteraService, RendimientoAsesorCarteraService>();
            services.AddScoped<IInicializacionCarteraService, InicializacionCarteraService>();
            services.AddScoped<IRendimientoCampanaCarteraService, RendimientoCampanaCarteraService>();
            services.AddScoped<IEvolucionCarteraService, EvolucionCarteraService>();
            services.AddScoped<IOpcionesFiltroCarteraService, OpcionesFiltroCarteraService>();
            services.AddScoped<IPanoramaCarteraService, PanoramaCarteraService>();
            services.AddScoped<IPromesasCarteraService, PromesasCarteraService>();
            services.AddScoped<IPromesasVencenHoyCarteraService, PromesasVencenHoyCarteraService>();
            services.AddScoped<ISeguimientoPromesasCarteraService, SeguimientoPromesasCarteraService>();
            services.AddScoped<IPromesasVencidasCarteraService, PromesasVencidasCarteraService>();
            services.AddScoped<IResumenCarteraService, ResumenCarteraService>();
            services.AddScoped<IRendimientoSupervisorCarteraService, RendimientoSupervisorCarteraService>();
            services.AddScoped<IAvanceMetaCarteraService, AvanceMetaCarteraService>();
        }
    }
}