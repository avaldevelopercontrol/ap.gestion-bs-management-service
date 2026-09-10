using System.Threading.RateLimiting;
using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
using GesMgmt.WebAPI.Controllers.Analytics;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using NLog;
using NLog.Web;
using GesMgmt.Infraestructure;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.WebAPI;
using System.Text.Json.Serialization;


var logger = NLog.LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
logger.Debug("La aplicacion se ha iniciado.");

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Host.UseNLog();

// Add services to the container.

// Memoria cache
builder.Services.AddMemoryCache();



//builder.Services.AddControllers();

//builder.Services.AddControllers()
//    .AddJsonOptions(options =>
//    {
//        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
//    });
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
    });

builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    
    c.SwaggerDoc("v1", new() { Title = "Gestión API", Version = "v1" });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://192.168.100.91:8080",
                "http://192.168.100.91:8090",
                "http://localhost:8080",
                "http://localhost:8090",
                "http://localhost:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// Add Infraestructure services
builder.Services.AddInfraestructure(builder.Configuration);

// Analytics consume la identidad del host sin registrar un esquema de autenticación propio.
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAnalyticsUserContext, HttpAnalyticsUserContext>();

var portfolioPerformance = builder.Configuration
    .GetSection(PortfolioControlCenterPerformanceOptions.SectionName)
    .Get<PortfolioControlCenterPerformanceOptions>()
    ?? new PortfolioControlCenterPerformanceOptions();

portfolioPerformance.Validate();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status503ServiceUnavailable;
    options.AddConcurrencyLimiter(
        AnalyticsControllerBase.PortfolioConcurrencyPolicyName,
        limiter =>
        {
            limiter.PermitLimit = portfolioPerformance.MaxConcurrentRequests;
            limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            limiter.QueueLimit = portfolioPerformance.QueueLimit;
        });
});

builder.Services.AddRequestTimeouts(options =>
{
    options.AddPolicy(
        AnalyticsControllerBase.PortfolioRequestTimeoutPolicyName,
        new RequestTimeoutPolicy
        {
            Timeout = TimeSpan.FromSeconds(portfolioPerformance.RequestTimeoutSeconds),
            TimeoutStatusCode = StatusCodes.Status503ServiceUnavailable
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("../swagger/v1/swagger.json", "Gestión API v1"));
//}

app.UseHttpsRedirection();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseRouting();
app.UseCors("ReactPolicy");

app.UseRequestTimeouts();
app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{
}
