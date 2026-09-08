using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence.Analytics;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

public static class AnalyticsOptionConfigSql
{
    public const string Exists = """
        SELECT CASE
            WHEN EXISTS
            (
                SELECT 1
                FROM analytics_access.option_config
                WHERE option_id = @OptionId
            ) THEN 1
            ELSE 0
        END;
        """;

    public const string GetAll = """
        SELECT
            option_id AS OptionId,
            option_code AS OptionCode,
            option_name AS OptionName
        FROM analytics_access.option_config
        WHERE is_active = 1
        ORDER BY option_id;
        """;

    public const string Update = """
        UPDATE analytics_access.option_config
        SET
            option_code = @OptionCode,
            option_name = @OptionName,
            is_active = @IsActive,
            updated_by = @UserId,
            updated_at = SYSUTCDATETIME()
        WHERE option_id = @OptionId;
        """;

    public const string InsertMissing = """
        INSERT INTO analytics_access.option_config
        (
            option_id,
            option_code,
            option_name,
            is_active,
            created_by,
            created_at
        )
        SELECT
            @OptionId,
            @OptionCode,
            @OptionName,
            @IsActive,
            @UserId,
            SYSUTCDATETIME()
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM analytics_access.option_config WITH (UPDLOCK, HOLDLOCK)
            WHERE option_id = @OptionId
        );
        """;

}
