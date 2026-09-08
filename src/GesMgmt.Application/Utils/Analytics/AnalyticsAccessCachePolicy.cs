using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.Utils.Analytics;

public static class AnalyticsAccessCachePolicy
{
    // Solo configuración compartida y de baja volatilidad. Los permisos de usuario,
    // scopes de cliente/grupo y datos SISGES se mantienen fuera de cache.
    public static readonly TimeSpan ConfigurationDuration =
        TimeSpan.FromSeconds(30);

    public static readonly TimeSpan CatalogDuration =
        TimeSpan.FromMinutes(1);
}
