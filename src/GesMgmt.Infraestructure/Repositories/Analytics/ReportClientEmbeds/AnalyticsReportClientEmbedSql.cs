using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence.Analytics;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

public static class AnalyticsReportClientEmbedSql
{
    public const string GetActiveForOption = """
        SELECT
            crm_client_id AS CrmClientId,
            report_client_value AS ReportClientValue,
            embed_url AS EmbedUrl
        FROM analytics_access.power_bi_report_client_embed
        WHERE option_id = @OptionId
          AND is_active = 1
        ORDER BY
            report_client_value,
            crm_client_id;
        """;

    public const string UpdatePublication = """
        UPDATE analytics_access.power_bi_report_client_embed
        SET
            embed_url = @EmbedUrl,
            is_active = 1,
            updated_by = @UpdatedBy,
            updated_at = SYSUTCDATETIME()
        WHERE option_id = @OptionId
          AND crm_client_id = @CrmClientId
          AND report_client_value = @ReportClientValue;
        """;

    public const string InsertPublication = """
        INSERT INTO analytics_access.power_bi_report_client_embed
        (
            option_id,
            crm_client_id,
            report_client_value,
            embed_url,
            is_active,
            created_by,
            updated_by
        )
        SELECT
            @OptionId,
            @CrmClientId,
            @ReportClientValue,
            @EmbedUrl,
            1,
            @UpdatedBy,
            @UpdatedBy
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM analytics_access.power_bi_report_client_embed WITH (UPDLOCK, HOLDLOCK)
            WHERE option_id = @OptionId
              AND crm_client_id = @CrmClientId
              AND report_client_value = @ReportClientValue
        );
        """;

    public const string DeactivatePublication = """
        UPDATE analytics_access.power_bi_report_client_embed
        SET
            is_active = 0,
            updated_by = @UpdatedBy,
            updated_at = SYSUTCDATETIME()
        WHERE option_id = @OptionId
          AND crm_client_id = @CrmClientId
          AND report_client_value = @ReportClientValue
          AND is_active = 1;
        """;
}
