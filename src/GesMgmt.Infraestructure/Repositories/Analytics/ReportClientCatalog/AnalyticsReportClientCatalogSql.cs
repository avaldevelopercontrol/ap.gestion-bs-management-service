using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence.Analytics;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

public static class AnalyticsReportClientCatalogSql
{
    public const string Current = """
        SELECT
            catalog.crm_client_id AS CrmClientId,
            catalog.report_client_value AS ReportClientValue
        FROM analytics_access.power_bi_report_client_catalog AS catalog
        WHERE catalog.option_id = @OptionId
        ORDER BY
            catalog.report_client_value,
            catalog.crm_client_id;
        """;
}
