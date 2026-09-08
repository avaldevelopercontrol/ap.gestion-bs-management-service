using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence.Analytics;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

public static class AnalyticsOptionClientScopeSql
{
    public const string GetClients = """
        SELECT crm_client_id
        FROM analytics_access.option_client_scope
        WHERE option_id = @OptionId
          AND is_active = 1
        ORDER BY crm_client_id;
        """;

    public const string GetInheritedClientIds = """
        SELECT ocs.crm_client_id
        FROM analytics_access.option_client_scope AS ocs
        INNER JOIN analytics_access.option_config AS oc
            ON oc.option_id = ocs.option_id
           AND oc.is_active = 1
        WHERE ocs.option_id = @OptionId
          AND ocs.is_active = 1
        ORDER BY ocs.crm_client_id;
        """;

    public const string GetInheritedClients = """
        SELECT
            ocs.crm_client_id AS CrmClientId,
            COALESCE(
                NULLIF(LTRIM(RTRIM(c.client_name)), ''),
                NULLIF(LTRIM(RTRIM(c.client_code)), ''),
                CONCAT('Cartera ', CONVERT(VARCHAR(20), ocs.crm_client_id))
            ) AS Name
        FROM analytics_access.option_client_scope AS ocs
        INNER JOIN analytics_access.option_config AS oc
            ON oc.option_id = ocs.option_id
           AND oc.is_active = 1
        LEFT JOIN analytics.dim_client AS c
            ON c.crm_client_id = ocs.crm_client_id
        WHERE ocs.option_id = @OptionId
          AND ocs.is_active = 1
        ORDER BY ocs.crm_client_id;
        """;

    public const string IsInheritedClient = """
        SELECT TOP (1) 1
        FROM analytics_access.option_client_scope AS ocs
        INNER JOIN analytics_access.option_config AS oc
            ON oc.option_id = ocs.option_id
           AND oc.is_active = 1
        WHERE ocs.option_id = @OptionId
          AND ocs.crm_client_id = @ClientId
          AND ocs.is_active = 1;
        """;

    public const string GetAuthorizedClientIds = """
        SELECT ocs.crm_client_id
        FROM analytics_access.option_client_scope AS ocs
        INNER JOIN analytics_access.option_config AS oc
            ON oc.option_id = ocs.option_id
           AND oc.is_active = 1
        WHERE ocs.option_id = @OptionId
          AND ocs.is_active = 1
          AND EXISTS
          (
              SELECT 1
              FROM analytics_access.user_option_scope AS uos
              WHERE uos.user_id = @UserId
                AND uos.option_id = ocs.option_id
                AND uos.is_active = 1
          )
        ORDER BY ocs.crm_client_id;
        """;

    public const string GetAuthorizedClients = """
        SELECT
            ocs.crm_client_id AS CrmClientId,
            COALESCE(
                NULLIF(LTRIM(RTRIM(c.client_name)), ''),
                NULLIF(LTRIM(RTRIM(c.client_code)), ''),
                CONCAT('Cartera ', CONVERT(VARCHAR(20), ocs.crm_client_id))
            ) AS Name
        FROM analytics_access.option_client_scope AS ocs
        INNER JOIN analytics_access.option_config AS oc
            ON oc.option_id = ocs.option_id
           AND oc.is_active = 1
        LEFT JOIN analytics.dim_client AS c
            ON c.crm_client_id = ocs.crm_client_id
        WHERE ocs.option_id = @OptionId
          AND ocs.is_active = 1
          AND EXISTS
          (
              SELECT 1
              FROM analytics_access.user_option_scope AS uos
              WHERE uos.user_id = @UserId
                AND uos.option_id = ocs.option_id
                AND uos.is_active = 1
          )
        ORDER BY ocs.crm_client_id;
        """;

    public const string IsAuthorizedClient = """
        SELECT TOP (1) 1
        FROM analytics_access.option_client_scope AS ocs
        INNER JOIN analytics_access.option_config AS oc
            ON oc.option_id = ocs.option_id
           AND oc.is_active = 1
        WHERE ocs.option_id = @OptionId
          AND ocs.crm_client_id = @ClientId
          AND ocs.is_active = 1
          AND EXISTS
          (
              SELECT 1
              FROM analytics_access.user_option_scope AS uos
              WHERE uos.user_id = @UserId
                AND uos.option_id = ocs.option_id
                AND uos.is_active = 1
          );
        """;

    public const string GetActiveScopes = """
        WITH requested_options AS
        (
            SELECT DISTINCT TRY_CONVERT(INT, [value]) AS option_id
            FROM OPENJSON(@OptionIdsJson)
            WHERE TRY_CONVERT(INT, [value]) > 0
        )
        SELECT
            scope.option_id AS OptionId,
            scope.crm_client_id AS CrmClientId
        FROM analytics_access.option_client_scope AS scope
        INNER JOIN requested_options AS requested
            ON requested.option_id = scope.option_id
        INNER JOIN analytics_access.option_config AS oc
            ON oc.option_id = scope.option_id
           AND oc.is_active = 1
        WHERE scope.is_active = 1
        ORDER BY
            scope.option_id,
            scope.crm_client_id;
        """;

    public const string Replace = """
        DECLARE @RequestedClients TABLE
        (
            crm_client_id INT NOT NULL PRIMARY KEY
        );

        INSERT INTO @RequestedClients (crm_client_id)
        SELECT DISTINCT TRY_CONVERT(INT, [value])
        FROM OPENJSON(@ClientIdsJson)
        WHERE TRY_CONVERT(INT, [value]) > 0;

        UPDATE analytics_access.option_client_scope
        SET
            is_active = 0,
            updated_by = @UserId,
            updated_at = SYSUTCDATETIME()
        WHERE option_id = @OptionId
          AND is_active = 1;

        UPDATE target
        SET
            target.is_active = 1,
            target.updated_by = @UserId,
            target.updated_at = SYSUTCDATETIME()
        FROM analytics_access.option_client_scope AS target
        INNER JOIN @RequestedClients AS requested
            ON requested.crm_client_id = target.crm_client_id
        WHERE target.option_id = @OptionId;

        INSERT INTO analytics_access.option_client_scope
        (
            option_id,
            crm_client_id,
            is_active,
            created_by,
            created_at
        )
        SELECT
            @OptionId,
            requested.crm_client_id,
            1,
            @UserId,
            SYSUTCDATETIME()
        FROM @RequestedClients AS requested
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM analytics_access.option_client_scope AS target
                WITH (UPDLOCK, HOLDLOCK)
            WHERE target.option_id = @OptionId
              AND target.crm_client_id = requested.crm_client_id
        );
        """;
}
