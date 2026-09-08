using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence.Analytics;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

public static class AnalyticsReportClientScopeSql
{
    public const string HasAnyScope = """
        SELECT COUNT(1)
        FROM analytics_access.power_bi_report_client_scope
        WHERE option_id = @OptionId
          AND is_active = 1;
        """;

    public const string GetMappings = """
        SELECT
            crm_client_id AS CrmClientId,
            report_client_value AS ReportClientValue,
            sisges_group_id AS SisgesGroupId
        FROM analytics_access.power_bi_report_client_scope
        WHERE option_id = @OptionId
          AND is_active = 1
        ORDER BY
            report_client_value,
            crm_client_id,
            sisges_group_id;
        """;

    public const string GetOptionIdsWithActiveScope = """
        WITH requested_options AS
        (
            SELECT DISTINCT TRY_CONVERT(INT, [value]) AS option_id
            FROM OPENJSON(@OptionIdsJson)
            WHERE TRY_CONVERT(INT, [value]) > 0
        )
        SELECT DISTINCT scope.option_id
        FROM analytics_access.power_bi_report_client_scope AS scope
        INNER JOIN requested_options AS requested
            ON requested.option_id = scope.option_id
        WHERE scope.is_active = 1
        ORDER BY scope.option_id;
        """;

    public const string ReplaceClientGroups = """
        DECLARE @RequestedGroups TABLE
        (
            sisges_group_id INT NOT NULL PRIMARY KEY
        );

        INSERT INTO @RequestedGroups (sisges_group_id)
        SELECT DISTINCT TRY_CONVERT(INT, [value])
        FROM OPENJSON(@GroupIdsJson)
        WHERE TRY_CONVERT(INT, [value]) > 0;

        UPDATE analytics_access.power_bi_report_client_scope
        SET
            is_active = 0,
            updated_by = @UpdatedBy,
            updated_at = SYSUTCDATETIME()
        WHERE option_id = @OptionId
          AND crm_client_id = @CrmClientId
          AND report_client_value = @ReportClientValue
          AND is_active = 1;

        UPDATE target
        SET
            target.is_active = 1,
            target.updated_by = @UpdatedBy,
            target.updated_at = SYSUTCDATETIME()
        FROM analytics_access.power_bi_report_client_scope AS target
        INNER JOIN @RequestedGroups AS requested
            ON requested.sisges_group_id = target.sisges_group_id
        WHERE target.option_id = @OptionId
          AND target.crm_client_id = @CrmClientId
          AND target.report_client_value = @ReportClientValue;

        INSERT INTO analytics_access.power_bi_report_client_scope
        (
            option_id,
            crm_client_id,
            report_client_value,
            sisges_group_id,
            is_active,
            created_by,
            updated_by
        )
        SELECT
            @OptionId,
            @CrmClientId,
            @ReportClientValue,
            requested.sisges_group_id,
            1,
            @UpdatedBy,
            @UpdatedBy
        FROM @RequestedGroups AS requested
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM analytics_access.power_bi_report_client_scope AS target
                WITH (UPDLOCK, HOLDLOCK)
            WHERE target.option_id = @OptionId
              AND target.crm_client_id = @CrmClientId
              AND target.report_client_value = @ReportClientValue
              AND target.sisges_group_id = requested.sisges_group_id
        );
        """;
}
