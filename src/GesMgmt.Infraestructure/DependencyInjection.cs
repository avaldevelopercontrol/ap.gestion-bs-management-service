using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Deudor;
using GesMgmt.Application.Interfaces.Direccion;
using GesMgmt.Application.Interfaces.Email;
using GesMgmt.Application.Interfaces.Gestion;
using GesMgmt.Application.Interfaces.Telefono;
using GesMgmt.Application.Interfaces.Usuario;
using GesMgmt.Application.Services;
using GesMgmt.Application.Services.Deudor;
using GesMgmt.Application.Services.Direccion;
using GesMgmt.Application.Services.Email;
using GesMgmt.Application.Services.Gestion;
using GesMgmt.Application.Services.Telefono;
using GesMgmt.Application.Services.Usuario;
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

            services.AddDbContext<AvalDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Memoria
            services.AddMemoryCache();

            // Repositories
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<Iav_DocxCobrarRepository, av_DocxCobrarRepository>();
            services.AddScoped<IValidationMessageRepository, ValidationMessageRespository>();

            // Services
            services.AddScoped<IDeudorService, DeudorService>();
            services.AddScoped<IDireccionService, DireccionService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IGestionService, GestionService>();
            services.AddScoped<ITelefonoService, TelefonoService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            
            services.AddScoped<IValidationMessageService, ValidationMessageService>();

            // Logger
            services.AddScoped(typeof(IAppLogger), typeof(LoggerAdapter));

            return services;
        }
    }
}