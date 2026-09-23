using System.Reflection;
using System.Text.RegularExpressions;
using GesMgmt.WebAPI.Controllers.Analitica;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace GesMgmt.UnitTests.Analitica.Architecture;

public sealed class AnaliticaRouteContratoTests
{
    [Fact]
    public void Controllers_MantienenLasTreintaYNueveOperacionesPublicasDeAnalitica()
    {
        var actual = typeof(AnaliticaControllerBase).Assembly
            .GetTypes()
            .Where(type =>
                !type.IsAbstract &&
                typeof(AnaliticaControllerBase).IsAssignableFrom(type))
            .SelectMany(GetOperations)
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();

        var expected = new[]
        {
            "GET /v1/Analitica/Acceso/Opciones",
            "GET /v1/Analitica/Acceso/Opciones/{idOpcion}/Clientes",
            "GET /v1/Analitica/Acceso/Opciones/{idOpcion}/Grupos",
            "GET /v1/Analitica/Acceso/Opciones/{idOpcion}/ConfiguracionPowerBi",
            "GET /v1/Analitica/Acceso/Opciones/{idOpcion}/IncrustacionesReporteCliente",
            "GET /v1/Analitica/Acceso/Opciones/{idOpcion}/Usuarios",
            "GET /v1/Analitica/Acceso/Usuario/Opciones",
            "GET /v1/Analitica/Acceso/Usuario/Opciones/{idOpcion}/AlcancesCliente",
            "GET /v1/Analitica/Acceso/Usuario/Opciones/{idOpcion}/Clientes",
            "GET /v1/Analitica/Acceso/Usuario/Opciones/{idOpcion}/AlcancesGrupo",
            "GET /v1/Analitica/Acceso/Usuario/Opciones/{idOpcion}/ContextoVisorPowerBi",
            "GET /v1/Analitica/Acceso/Usuario/Opciones/{idOpcion}/IncrustacionReporteCliente",
            "GET /v1/Analitica/Acceso/Usuario/Opciones/{idOpcion}/ClientesReporte",
            "GET /v1/Analitica/Acceso/Usuario/AccesoPowerBi",
            "PATCH /v1/Analitica/Acceso/Opciones/{idOpcion}/ConfiguracionPowerBi",
            "PUT /v1/Analitica/Acceso/Opciones/{idOpcion}",
            "PUT /v1/Analitica/Acceso/Opciones/{idOpcion}/Clientes",
            "PUT /v1/Analitica/Acceso/Opciones/{idOpcion}/Grupos",
            "PUT /v1/Analitica/Acceso/Opciones/{idOpcion}/IncrustacionesReporteCliente",
            "PUT /v1/Analitica/Acceso/Opciones/{idOpcion}/Usuarios",
            "GET /v1/Analitica/CentroControlCartera/RendimientoAsesor",
            "GET /v1/Analitica/CentroControlCartera/Inicializacion",
            "GET /v1/Analitica/CentroControlCartera/RendimientoCampana",
            "GET /v1/Analitica/CentroControlCartera/Evolucion",
            "GET /v1/Analitica/CentroControlCartera/Evolucion/Comparativa",
            "GET /v1/Analitica/CentroControlCartera/OpcionesFiltro",
            "GET /v1/Analitica/CentroControlCartera/Panorama",
            "GET /v1/Analitica/CentroControlCartera/Promesas",
            "GET /v1/Analitica/CentroControlCartera/Promesas/VenceHoy",
            "GET /v1/Analitica/CentroControlCartera/Promesas/Seguimiento",
            "GET /v1/Analitica/CentroControlCartera/Promesas/Vencidas",
            "GET /v1/Analitica/CentroControlCartera/Resumen",
            "GET /v1/Analitica/CentroControlCartera/RendimientoSupervisor",
            "GET /v1/Analitica/CentroControlCartera/AvanceMeta",
            "GET /v1/Analitica/PowerBi/Sesiones/Panel",
            "GET /v1/Analitica/PowerBi/Sesiones/{idSesion}",
            "POST /v1/Analitica/PowerBi/Sesiones",
            "PUT /v1/Analitica/PowerBi/Sesiones/{idSesion}/Actividad",
            "POST /v1/Analitica/PowerBi/Sesiones/{idSesion}/Cerrar"
        }.OrderBy(value => value, StringComparer.Ordinal).ToArray();

        Assert.Equal(expected, actual);
    }

    private static IEnumerable<string> GetOperations(Type controllerType)
    {
        var controllerRoute = controllerType
            .GetCustomAttributes<RouteAttribute>(inherit: true)
            .Single()
            .Template;

        foreach (var method in controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public))
        {
            foreach (var httpAttribute in method.GetCustomAttributes<HttpMethodAttribute>(inherit: true))
            {
                var route = Combine(controllerRoute, httpAttribute.Template);

                foreach (var httpMethod in httpAttribute.HttpMethods)
                {
                    yield return $"{httpMethod.ToUpperInvariant()} {route}";
                }
            }
        }
    }

    private static string Combine(string? controllerRoute, string? actionRoute)
    {
        var combined = string.Join(
            '/',
            new[] { controllerRoute, actionRoute }
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value!.Trim('/')));

        combined = Regex.Replace(combined, @":(?:int|guid)(?=})", string.Empty);
        return "/" + combined;
    }
}
