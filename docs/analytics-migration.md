# Integración de Analytics en GesMgmt

## Estado final

Analytics quedó integrado dentro de `GesMgmt.WebAPI` respetando las capas y el patrón de
persistencia del backend existente. No existe un proyecto ejecutable Analytics separado ni
una infraestructura Dapper paralela.

## Reglas arquitectónicas

1. `GesMgmt.WebAPI` es el único host ASP.NET Core.
2. `GesMgmt.Domain` contiene entidades y contratos.
3. `GesMgmt.Application` contiene casos de uso, DTOs y reglas de aplicación.
4. `GesMgmt.Infraestructure` implementa repositories y persistencia con EF Core.
5. `GesMgmt.WebAPI` mantiene controllers y composición/DI; no contiene una carpeta genérica
   `Services` para infraestructura Analytics.
6. Analytics no agrega un esquema de autenticación propio ni el header de desarrollo
   `X-Sisges-User-Id`.
7. Los endpoints y contratos públicos de Analytics se conservan salvo cambio funcional
   explícitamente aprobado.
8. Los repositories Analytics no se agregan al `IUnitOfWork` legacy: `AvalDbContext` y
   `AnalyticsDbContext` representan bases de datos distintas.

## Contextos de datos

### AvalDbContext

- Connection string: `ConnectionStrings:AvalCobConnection`.
- Base: `aval_cob`.
- También es reutilizado por Analytics cuando necesita consultar datos SISGES existentes.

### AnalyticsDbContext

- Connection string: `ConnectionStrings:AvalAnalyticsConnection`.
- Base: `aval_analytics`.
- Modela `analytics_access`, dimensiones, facts y vistas utilizadas por Analytics y Portfolio
  Control Center.
- `AnalyticsDatabase:CommandTimeoutSeconds` es opcional; default 15 segundos, rango 1-120.

## Migración de persistencia

La primera integración de Analytics utilizaba Dapper, `Microsoft.Data.SqlClient`, ejecutores
SQL y clases `*Sql.cs`. Esa estrategia fue retirada para volver al patrón EF Core del proyecto.

Quedaron migrados a EF Core:

- acceso SISGES de Analytics;
- opciones, usuarios, clientes y grupos de `analytics_access`;
- configuración Power BI y publicaciones por cliente;
- Bootstrap y FilterOptions de Portfolio Control Center;
- Overview, Summary y Target Progress;
- Promises, Due Today y Overdue;
- Evolution;
- Advisor, Supervisor y Campaign Performance.

No quedan archivos `*Sql.cs` ni dependencias Dapper en los proyectos de aplicación o
infraestructura.

### Auditoría de scopes

Las tablas:

- `analytics_access.user_option_scope_audit`;
- `analytics_access.option_client_scope_audit`;
- `analytics_access.option_group_scope_audit`;

se escriben mediante `ExecuteSqlInterpolatedAsync` del propio `AnalyticsDbContext`, dentro de
la misma transacción EF Core que actualiza los scopes. El código fuente disponible no contiene
el DDL ni la PK real de esas tablas; por ello no se inventa una clave EF únicamente para
eliminar esos `INSERT` parametrizados. No existen `SELECT`, `UPDATE` ni `DELETE` SQL manuales
para la lógica funcional de Analytics.

## Configuración local

`src/GesMgmt.WebAPI/appsettings.json` es parte del proyecto y declara las dos connection
strings. Las credenciales deben adecuarse al entorno y pueden sobrescribirse con mecanismos
estándar de configuración ASP.NET Core.

## Validación en Windows

Desde Visual Studio Community 2026:

1. Abrir `API.BS.GestionManagement.slnx`.
2. Seleccionar `GesMgmt.WebAPI` como Startup Project.
3. Ejecutar `Build > Rebuild Solution`.
4. Ejecutar el perfil HTTPS y validar los endpoints desde Swagger.

Desde PowerShell:

```powershell
dotnet restore .\API.BS.GestionManagement.slnx
dotnet build .\API.BS.GestionManagement.slnx -c Release
dotnet test .\src\GesMgmt.UnitTests\GesMgmt.UnitTests.csproj -c Release
```

## Gate de identidad

La integración no inventa autenticación para Gestión. Antes de habilitar Analytics en un
entorno productivo, el host/plataforma debe entregar un `ClaimsPrincipal` autenticado con el
identificador SISGES que consume `IAnalyticsUserContext`.
