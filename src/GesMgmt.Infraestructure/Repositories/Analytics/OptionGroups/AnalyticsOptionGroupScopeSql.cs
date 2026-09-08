using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence.Analytics;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

public static class AnalyticsOptionGroupScopeSql
{
    public const string HasAnyScope = """
        SELECT COUNT(1)
        FROM analytics_access.option_group_scope
        WHERE option_id = @OptionId;
        """;

    public const string GetGroups = """
        SELECT sisges_group_id
        FROM analytics_access.option_group_scope
        WHERE option_id = @OptionId
          AND is_active = 1
        ORDER BY sisges_group_id;
        """;

    public const string GetScopes = """
        WITH requested_options AS
        (
            SELECT DISTINCT TRY_CONVERT(INT, [value]) AS option_id
            FROM OPENJSON(@OptionIdsJson)
            WHERE TRY_CONVERT(INT, [value]) > 0
        )
        SELECT
            scope.option_id AS OptionId,
            scope.sisges_group_id AS SisgesGroupId,
            scope.is_active AS IsActive
        FROM analytics_access.option_group_scope AS scope
        INNER JOIN requested_options AS requested
            ON requested.option_id = scope.option_id
        ORDER BY
            scope.option_id,
            scope.sisges_group_id;
        """;

    public const string Replace = """
        DECLARE @RequestedGroups TABLE
        (
            sisges_group_id INT NOT NULL PRIMARY KEY
        );

        INSERT INTO @RequestedGroups (sisges_group_id)
        SELECT DISTINCT TRY_CONVERT(INT, [value])
        FROM OPENJSON(@GroupIdsJson)
        WHERE TRY_CONVERT(INT, [value]) > 0;

        UPDATE analytics_access.option_group_scope
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
        FROM analytics_access.option_group_scope AS target
        INNER JOIN @RequestedGroups AS requested
            ON requested.sisges_group_id = target.sisges_group_id
        WHERE target.option_id = @OptionId;

        INSERT INTO analytics_access.option_group_scope
        (
            option_id,
            sisges_group_id,
            is_active,
            created_by,
            created_at,
            updated_by,
            updated_at
        )
        SELECT
            @OptionId,
            requested.sisges_group_id,
            1,
            @UserId,
            SYSUTCDATETIME(),
            @UserId,
            SYSUTCDATETIME()
        FROM @RequestedGroups AS requested
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM analytics_access.option_group_scope AS target
                WITH (UPDLOCK, HOLDLOCK)
            WHERE target.option_id = @OptionId
              AND target.sisges_group_id = requested.sisges_group_id
        );
        """;
}
