using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.Services.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.Validators.Analytics;

public static class AnalyticsReportClientPublicationUpdateValidator
{
    private const int MaxReportClientValueLength = 150;
    private const int MaxEmbedUrlLength = 2048;

    public static AnalyticsReportClientPublicationValidationResult Validate(
        IReadOnlyList<UpdateAnalyticsOptionReportClientEmbed>? publications,
        IReadOnlyList<AnalyticsReportClientConfiguration> configurations,
        bool allowPublishToWeb = true)
    {
        var configurationByKey = configurations.ToDictionary(
            configuration => BuildKey(
                configuration.ClientId,
                configuration.Name),
            StringComparer.OrdinalIgnoreCase);

        var requestedPublications = publications ?? [];
        var seenKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var normalizedUpdates = new List<AnalyticsReportClientPublicationUpdate>(
            requestedPublications.Count);

        foreach (var publication in requestedPublications)
        {
            var name = publication.Name?.Trim() ?? string.Empty;

            if (
                publication.ClientId <= 0 ||
                name.Length == 0 ||
                name.Length > MaxReportClientValueLength)
            {
                return Invalid(
                    "Configuración por cartera inválida",
                    "Cada configuración debe indicar un clientId positivo y una cartera válida.");
            }

            var key = BuildKey(publication.ClientId, name);

            if (!seenKeys.Add(key))
            {
                return Invalid(
                    "Configuración por cartera duplicada",
                    $"La cartera '{name}' fue enviada más de una vez.");
            }

            if (!configurationByKey.TryGetValue(key, out var configuration))
            {
                return Invalid(
                    "Cartera desconocida",
                    $"La cartera '{name}' no pertenece al catálogo ni a la configuración de esta opción Analytics.");
            }

            if (!configuration.IsAvailable)
            {
                return Invalid(
                    "Cartera no disponible",
                    $"La cartera '{configuration.Name}' ya no está disponible actualmente en la fuente del reporte.");
            }

            var candidateGroupIds = configuration.CandidateGroups
                .Select(group => group.GroupId)
                .ToHashSet();

            int[]? requestedGroupIds = null;

            if (publication.GroupIds is not null)
            {
                if (publication.GroupIds.Any(groupId => groupId <= 0))
                {
                    return Invalid(
                        "Grupo de cartera inválido",
                        $"Todos los grupos seleccionados para '{configuration.Name}' deben ser enteros positivos.");
                }

                requestedGroupIds = publication.GroupIds
                    .Distinct()
                    .OrderBy(groupId => groupId)
                    .ToArray();

                if (requestedGroupIds.Any(groupId => !candidateGroupIds.Contains(groupId)))
                {
                    return Invalid(
                        "Grupo fuera del cliente",
                        $"Uno o más grupos seleccionados para '{configuration.Name}' no están activos o no pertenecen al cliente SISGES {configuration.ClientId}.");
                }
            }

            var effectiveGroupIds = requestedGroupIds ?? configuration.GroupIds;
            var rawUrl = publication.EmbedUrl?.Trim() ?? string.Empty;
            string? normalizedUrl = null;

            if (rawUrl.Length > 0)
            {
                if (!allowPublishToWeb)
                {
                    return Invalid(
                        "Publish to web deshabilitado",
                        "La API no permite guardar publicaciones públicas de Power BI en este entorno.");
                }

                if (
                    rawUrl.Length > MaxEmbedUrlLength ||
                    !PowerBiPublishToWebUrl.TryNormalize(rawUrl, out var parsedUrl))
                {
                    return Invalid(
                        "URL Publish to web inválida",
                        $"La URL configurada para '{configuration.Name}' debe usar https://app.powerbi.com/view?r=...");
                }

                normalizedUrl = parsedUrl;
            }

            if (normalizedUrl is not null && effectiveGroupIds.Count == 0)
            {
                return Invalid(
                    "Acceso de cartera pendiente",
                    $"Seleccione al menos un grupo SISGES para '{configuration.Name}' antes de habilitar su publicación.");
            }

            normalizedUpdates.Add(
                new AnalyticsReportClientPublicationUpdate(
                    configuration.ClientId,
                    configuration.Name,
                    requestedGroupIds,
                    normalizedUrl));
        }

        return new AnalyticsReportClientPublicationValidationResult(
            normalizedUpdates,
            null);
    }

    private static AnalyticsReportClientPublicationValidationResult Invalid(
        string title,
        string detail) =>
        new(
            Array.Empty<AnalyticsReportClientPublicationUpdate>(),
            new AnalyticsReportClientPublicationValidationError(
                title,
                detail));

    private static string BuildKey(int clientId, string name) =>
        AnalyticsReportClientConfigurationResolver.BuildKey(clientId, name);
}

public sealed record AnalyticsReportClientPublicationValidationResult(
    IReadOnlyList<AnalyticsReportClientPublicationUpdate> Updates,
    AnalyticsReportClientPublicationValidationError? Error);

public sealed record AnalyticsReportClientPublicationValidationError(
    string Title,
    string Detail);
