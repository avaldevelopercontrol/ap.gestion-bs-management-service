using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GesMgmt.Domain.Interfaces;
using GesMgmt.Infraestructure.Logger;
using GesMgmt.Infraestructure.Persistence;
using GesMgmt.Infraestructure.Repositories;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Services;
using GesMgmt.Application.Services.Gestion;
using GesMgmt.Application.Interfaces.Gestion;
using GesMgmt.Application.Services.Telefono;
using GesMgmt.Application.Interfaces.Telefono;

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
            services.AddScoped<IGestionService, GestionService>();
            services.AddScoped<ITelefonoService, TelefonoService>();
            services.AddScoped<IValidationMessageService, ValidationMessageService>();

            // Logger
            services.AddScoped(typeof(IAppLogger), typeof(LoggerAdapter));

            return services;
        }
    }
}