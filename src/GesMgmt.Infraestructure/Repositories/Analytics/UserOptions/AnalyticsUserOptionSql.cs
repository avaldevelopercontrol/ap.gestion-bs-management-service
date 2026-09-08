using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence.Analytics;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

public static class AnalyticsUserOptionSql
{
    public const string HasAccess = """
        SELECT COUNT(1)
        FROM analytics_access.user_option_scope
        WHERE user_id = @UserId
          AND option_id = @OptionId
          AND is_active = 1;
        """;

    public const string GetUserOptions = """
        SELECT
            uos.option_id AS OptionId,
            oc.option_code AS OptionCode,
            oc.option_name AS OptionName
        FROM analytics_access.user_option_scope AS uos
        INNER JOIN analytics_access.option_config AS oc
            ON oc.option_id = uos.option_id
        WHERE uos.user_id = @UserId
          AND uos.is_active = 1
          AND oc.is_active = 1
        ORDER BY oc.option_id;
        """;

    public const string GetUsers = """
        SELECT user_id
        FROM analytics_access.user_option_scope
        WHERE option_id = @OptionId
          AND is_active = 1
        ORDER BY user_id;
        """;

    public const string Replace = """
        DECLARE @RequestedUsers TABLE
        (
            user_id INT NOT NULL PRIMARY KEY
        );

        INSERT INTO @RequestedUsers (user_id)
        SELECT DISTINCT TRY_CONVERT(INT, [value])
        FROM OPENJSON(@UserIdsJson)
        WHERE TRY_CONVERT(INT, [value]) > 0;

        UPDATE analytics_access.user_option_scope
        SET
            is_active = 0,
            updated_by = @AdminUserId,
            updated_at = SYSUTCDATETIME()
        WHERE option_id = @OptionId
          AND is_active = 1;

        UPDATE target
        SET
            target.is_active = 1,
            target.updated_by = @AdminUserId,
            target.updated_at = SYSUTCDATETIME()
        FROM analytics_access.user_option_scope AS target
        INNER JOIN @RequestedUsers AS requested
            ON requested.user_id = target.user_id
        WHERE target.option_id = @OptionId;

        INSERT INTO analytics_access.user_option_scope
        (
            user_id,
            option_id,
            is_active,
            created_by,
            created_at
        )
        SELECT
            requested.user_id,
            @OptionId,
            1,
            @AdminUserId,
            SYSUTCDATETIME()
        FROM @RequestedUsers AS requested
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM analytics_access.user_option_scope AS target
                WITH (UPDLOCK, HOLDLOCK)
            WHERE target.user_id = requested.user_id
              AND target.option_id = @OptionId
        );
        """;
}
