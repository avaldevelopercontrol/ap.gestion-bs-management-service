# API.BS.GestionManagement

Backend .NET 10 de Gestión. La funcionalidad que anteriormente se ejecutaba en
`Analytics.Api` está integrada en este mismo host ASP.NET Core y distribuida entre
`GesMgmt.Domain`, `GesMgmt.Application`, `GesMgmt.Infraestructure` y
`GesMgmt.WebAPI` de acuerdo con la arquitectura existente.

## Proyectos

- `GesMgmt.Domain`: entidades, constantes y contratos de persistencia.
- `GesMgmt.Application`: DTOs, interfaces, servicios, validadores y reglas de aplicación.
- `GesMgmt.Infraestructure`: EF Core legacy y persistencia Analytics con Dapper/SQL Server.
- `GesMgmt.WebAPI`: controllers y composition root único.
- `GesMgmt.UnitTests`: tests de regresión y compatibilidad del host consolidado.

## Analytics integrado

Las superficies públicas migradas conservan sus rutas:

- `/api/v1/analytics-access/*`
- `/api/v1/portfolio-control-center/*`
- `/health/live`
- `/health/ready`

Analytics no registra un esquema de autenticación propio. Consume exclusivamente una
identidad autenticada que el host proporcione mediante `HttpContext.User`. Los claims
admitidos por `IAnalyticsUserContext` son `sisges_user_id`, `user_id` y
`ClaimTypes.NameIdentifier`.

No existe soporte runtime para `X-Sisges-User-Id` ni para la autenticación Development
del proyecto anterior.

## Configuración de datos

El host utiliza:

- `ConnectionStrings:AvalCobConnection` para SISGES / `aval_cob`;
- `ConnectionStrings:Analytics` para `aval_analytics`.

No almacene connection strings ni credenciales en Git. En local puede usar el
`UserSecretsId` existente de `GesMgmt.WebAPI`:

```bash
dotnet user-secrets set --project src/GesMgmt.WebAPI \
  "ConnectionStrings:Analytics" \
  "<connection-string-a-aval_analytics>"
```

La conexión legacy `AvalCobConnection` debe continuar configurándose con el mecanismo
que ya utiliza Gestión.

## Verificación

```bash
dotnet restore API.BS.GestionManagement.slnx
dotnet build API.BS.GestionManagement.slnx
dotnet test src/GesMgmt.UnitTests/GesMgmt.UnitTests.csproj
```

Para validar el artefacto Release consolidado:

```bash
./scripts/verify-analytics-integration.sh
```

## Gate antes de producción

La migración no inventa ni reemplaza el mecanismo de autenticación de Gestión. Antes de
habilitar los endpoints Analytics en producción, el host o la plataforma de identidad
debe entregar un `ClaimsPrincipal` autenticado con un identificador SISGES válido. Sin
esa integración, los endpoints que requieren usuario responden `401` de forma deliberada.
