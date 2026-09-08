# Migración de Analytics dentro de GesMgmt

## Objetivo

Integrar la funcionalidad de `analytics-api` dentro de `GesMgmt.WebAPI` como un único host ASP.NET Core, respetando la arquitectura por capas existente de Gestión.

## Reglas de compatibilidad

1. `GesMgmt.WebAPI` será el único proyecto ejecutable al finalizar la migración.
2. El código migrado se distribuirá entre `GesMgmt.Domain`, `GesMgmt.Application`, `GesMgmt.Infraestructure` y `GesMgmt.WebAPI` respetando también las carpetas técnicas existentes (`DTOs`, `Interfaces`, `Services`, `Validators`, `Constants`, `Entities`, `Repositories`, `Persistence` y `Controllers`); no se creará una carpeta raíz `Analytics` que replique una arquitectura paralela dentro de cada proyecto.
3. No se migrará la autenticación propia de `Analytics.Api`, su `FallbackPolicy` ni el header de desarrollo `X-Sisges-User-Id`.
4. Analytics consumirá la identidad que proporcione el host mediante `IAnalyticsUserContext`.
5. La ausencia de identidad deberá producir el comportamiento de acceso correspondiente en los endpoints Analytics, sin modificar los endpoints legacy de Gestión.
6. Las rutas, payloads y códigos HTTP públicos de Analytics se conservarán salvo cambio funcional aprobado explícitamente.
7. Dapper y `Microsoft.Data.SqlClient` permanecerán en Infrastructure para la persistencia Analytics; no se forzará su migración a EF Core.
8. Los repositories Analytics no se agregarán al `IUnitOfWork` legacy de Gestión.
9. La configuración local específica de autenticación de `Analytics.Api` no se trasladará a producción ni al host consolidado.
10. `Analytics` se utilizará únicamente como subcarpeta de una responsabilidad técnica cuando sea necesario agrupar funcionalidad, por ejemplo `Services/Analytics` o `Repositories/Analytics`.
11. Cada etapa de migración debe poder validarse de forma independiente antes de aplicar la siguiente.

## Etapas

- Stage 0: baseline, dependencias y frontera de identidad del host.
- Stage 1: infraestructura de base de datos, ejecutores Dapper, health checks y observabilidad de acceso SQL.
- Stage 2: Analytics Access y administración.
- Stage 3: Portfolio Control Center y protecciones de recursos.
- Stage 4: compatibilidad contractual, tests y consolidación final.


## Stage 1 - Infraestructura de datos

La infraestructura Analytics se integra bajo `GesMgmt.Infraestructure/Persistence/Analytics`; no se crea un proyecto ni una raíz arquitectónica paralela.

- `ConnectionStrings:AvalCobConnection` continúa siendo la conexión SISGES/`aval_cob` del host y también es reutilizada por las consultas Analytics que leen SISGES.
- `ConnectionStrings:Analytics` es la única conexión adicional requerida para `aval_analytics`. No se almacena ningún secreto en el repositorio.
- No se migra `DevelopmentBridge`, `AnalyticsDatabase:Provider`, `X-Sisges-User-Id` ni ningún esquema de autenticación de desarrollo.
- La ausencia de `ConnectionStrings:Analytics` no impide iniciar Gestión en este stage; `/health/ready` reportará la dependencia Analytics como `Unhealthy` hasta que la conexión sea configurada.
- `/health/live` comprueba únicamente que el proceso esté activo. `/health/ready` comprueba `aval_cob` y `aval_analytics` y devuelve descripciones seguras, sin exponer excepciones SQL.

Configuración local recomendada para la conexión Analytics, usando el `UserSecretsId` existente de `GesMgmt.WebAPI`:

```bash
dotnet user-secrets set --project src/GesMgmt.WebAPI \
  "ConnectionStrings:Analytics" "<connection-string-a-aval_analytics>"
```

## Stage 2 - Analytics Access y administración

La funcionalidad de `AnalyticsAccess` se distribuye dentro de las responsabilidades técnicas existentes de GesMgmt:

- contratos HTTP en `GesMgmt.Application/DTOs/Analytics`;
- interfaces de casos de uso en `GesMgmt.Application/Interfaces/Analytics`;
- servicios y reglas de aplicación en `GesMgmt.Application/Services/Analytics`;
- entidades y contratos de persistencia en `GesMgmt.Domain/Entities/Analytics` y `GesMgmt.Domain/Interfaces/Analytics`;
- implementaciones Dapper/SQL en `GesMgmt.Infraestructure/Repositories/Analytics`;
- endpoints HTTP convertidos de Minimal APIs a controllers en `GesMgmt.WebAPI/Controllers/Analytics`.

Se conservan las 20 operaciones públicas bajo `/api/v1/analytics-access` y sus contratos de respuesta. Las respuestas de esta superficie mantienen `Cache-Control: no-store`, `Pragma: no-cache`, `X-Trace-Id` y `ProblemDetails` para fallos controlados, sin aplicar ese comportamiento a las rutas legacy de Gestión.

Stage 2 no registra un esquema de autenticación. `IAnalyticsUserContext` solo consume una identidad autenticada que ya haya sido establecida por el host y busca un identificador positivo en los claims `sisges_user_id`, `user_id` o `ClaimTypes.NameIdentifier`. Si el host todavía no proporciona una identidad autenticada, los endpoints Analytics que requieran usuario responderán `401`; no se introduce un mecanismo alternativo inseguro para evitarlo.

Los endpoints administrativos requieren además que el identificador del usuario esté incluido en `AnalyticsAdministration:AdministratorUserIds`. `AnalyticsPowerBiSecurity:AllowPublishToWeb` conserva `true` como valor por defecto y puede deshabilitarse mediante configuración. Ninguna de estas opciones contiene secretos.

## Stage 3 - Portfolio Control Center

`PortfolioControlCenter` se migra siguiendo la organización interna de GesMgmt y deja de depender de Minimal APIs:

- contratos de entrada normalizados y modelos de consulta en `GesMgmt.Domain/Entities/Analytics/PortfolioControlCenter`;
- interfaces de persistencia en `GesMgmt.Domain/Interfaces/Analytics/PortfolioControlCenter`;
- contratos de respuesta en `GesMgmt.Application/DTOs/Analytics/PortfolioControlCenter`;
- interfaces y casos de uso en `GesMgmt.Application/Interfaces/Analytics/PortfolioControlCenter` y `GesMgmt.Application/Services/Analytics/PortfolioControlCenter`;
- Dapper/SQL en `GesMgmt.Infraestructure/Repositories/Analytics/PortfolioControlCenter`;
- cache de detalle en `GesMgmt.Infraestructure/Persistence/Analytics/Caching`;
- controllers HTTP en `GesMgmt.WebAPI/Controllers/Analytics/PortfolioControlCenter`.

Se conservan las 12 rutas públicas originales bajo `/api/v1/portfolio-control-center`, sus payloads, validaciones funcionales y códigos de error. Los controllers son adaptadores HTTP; la resolución de cartera autorizada, rango, Business Unit, contexto y consulta permanece en Application/Domain/Infrastructure según corresponda.

Las protecciones de recursos se registran únicamente para los controllers de Portfolio Control Center mediante una policy de concurrencia y una policy de timeout. Sus valores se obtienen de `PortfolioControlCenter:Performance` y, si la sección no existe, se conservan los defaults del backend original: 80 requests concurrentes, cola de 240, timeout de 60 segundos, cache de detalle de 30 segundos y hasta 512 entradas.

Stage 3 no introduce `AddAuthentication`, `UseAuthentication`, `FallbackPolicy`, `X-Sisges-User-Id` ni perfiles de autenticación Development. La autorización funcional reutiliza `IAnalyticsUserContext` y `IAnalyticsAccessService` migrados en los stages anteriores.

`AnalyticsRequestMiddleware` cubre ahora tanto `/api/v1/analytics-access` como `/api/v1/portfolio-control-center` para conservar `X-Trace-Id` y el tratamiento seguro de errores de datos. El `no-store` continúa limitado a Analytics Access; Portfolio Control Center conserva su política de cache de datos y no recibe headers `no-store` de forma artificial.

## Stage 4 - Compatibilidad, tests y consolidación final

Stage 4 cierra la migración estructural y elimina la dependencia de un proyecto
`Analytics.Api` ejecutable separado. `GesMgmt.WebAPI` es el único host ASP.NET Core.

El proyecto `GesMgmt.UnitTests`, que todavía contenía referencias y tests heredados de
`SubMgmt`, se corrige para referenciar los cuatro proyectos reales de Gestión y se agrega
a la solución. Los tests iniciales de regresión cubren:

- inventario contractual de las 32 operaciones HTTP migradas;
- frontera de identidad y rechazo explícito de `X-Sisges-User-Id`;
- `401` fail-closed cuando el host no entrega una identidad autenticada;
- `ProblemDetails`, `X-Trace-Id` y headers `no-store` de Analytics Access;
- health checks live/ready sin depender de SQL real durante el test;
- defaults y validación de las protecciones de Portfolio Control Center;
- reglas de Business Unit, URL Publish to web y clasificación segura de fallos SQL.

El middleware Analytics completa además la compatibilidad de rutas inexistentes: un
`404` vacío bajo las superficies Analytics se transforma en `application/problem+json`
con `traceId`, sin aplicar `StatusCodePages` globales ni modificar los endpoints legacy.

Para release se incorpora `scripts/verify-analytics-integration.sh`. El script valida
higiene del repositorio, ausencia del antiguo proyecto ejecutable y de autenticación
Development, ejecuta build/tests Release y publica únicamente `GesMgmt.WebAPI`.
`appsettings.Development.json` se excluye del artefacto de publicación.

### Gate de identidad para producción

La consolidación de código queda terminada, pero la autenticación sigue siendo una
responsabilidad del host. No se migró el esquema de autenticación de `Analytics.Api` y
Gestión actualmente no crea un `ClaimsPrincipal` autenticado. Antes de habilitar Analytics
en producción se debe conectar `IAnalyticsUserContext` con la identidad autenticada real
del host/plataforma. Hasta entonces el comportamiento correcto de los endpoints que
requieren usuario es `401`; no se debe introducir un header temporal o un bypass para
simular producción.
