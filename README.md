# API.BS.GestionManagement

Backend .NET 10 de Gestión. Analytics está integrado en el mismo host ASP.NET Core y
respeta las capas existentes: `GesMgmt.Domain`, `GesMgmt.Application`,
`GesMgmt.Infraestructure` y `GesMgmt.WebAPI`.

## Proyectos

- `GesMgmt.Domain`: entidades, constantes y contratos de persistencia.
- `GesMgmt.Application`: DTOs, interfaces, servicios, validadores y reglas de aplicación.
- `GesMgmt.Infraestructure`: persistencia SQL Server mediante Entity Framework Core.
- `GesMgmt.WebAPI`: controllers y composition root único.
- `GesMgmt.UnitTests`: tests de regresión y compatibilidad.

## Persistencia

El backend usa un único patrón de acceso a datos: Entity Framework Core.

- `AvalDbContext` usa `ConnectionStrings:AvalCobConnection` para `aval_cob`.
- `AnalyticsDbContext` usa `ConnectionStrings:AvalAnalyticsConnection` para `aval_analytics`.
- Los repositories de Analytics no usan Dapper ni ejecutores SQL paralelos.
- Los repositories Analytics no forman parte del `IUnitOfWork` legacy porque trabajan con
  un `DbContext` y una base de datos distintos.

Las tablas de auditoría de scopes se escriben mediante `AnalyticsDbContext.Database.ExecuteSqlInterpolatedAsync`
dentro de la misma transacción EF Core. El repositorio no contiene el DDL/PK de esas tablas,
por lo que no se modelan con una clave ficticia.

## Configuración

`src/GesMgmt.WebAPI/appsettings.json` forma parte del proyecto y contiene la estructura base:

```json
{
  "ConnectionStrings": {
    "AvalCobConnection": "...",
    "AvalAnalyticsConnection": "..."
  }
}
```

Las credenciales reales deben sustituirse según el entorno. ASP.NET Core permite sobrescribir
estos valores mediante User Secrets, variables de entorno u otra fuente de configuración.

Opcionalmente puede configurarse `AnalyticsDatabase:CommandTimeoutSeconds`; el valor por
defecto es 15 segundos y el rango permitido es 1-120.

## Ejecución local en Windows

Abra `API.BS.GestionManagement.slnx` con Visual Studio Community 2026, establezca
`GesMgmt.WebAPI` como Startup Project, seleccione el perfil `https` y ejecute Rebuild Solution.
El perfil HTTPS usa `https://localhost:7143`.

También puede validar desde PowerShell:

```powershell
dotnet restore .\API.BS.GestionManagement.slnx
dotnet build .\API.BS.GestionManagement.slnx
dotnet test .\src\GesMgmt.UnitTests\GesMgmt.UnitTests.csproj
```

Para ejecutar la verificación final de integración en Windows:

```powershell
.\scripts\verify-analytics-integration.ps1
```

## Analytics integrado

Las superficies públicas permanecen bajo:

- `/api/v1/analytics-access/*`
- `/api/v1/portfolio-control-center/*`

Analytics no registra un esquema de autenticación propio. `IAnalyticsUserContext` consume la
identidad autenticada que entregue el host mediante `HttpContext.User`.
