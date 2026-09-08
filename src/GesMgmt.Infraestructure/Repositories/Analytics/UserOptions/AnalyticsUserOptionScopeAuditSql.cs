using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence.Analytics;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

public static class AnalyticsUserOptionScopeAuditSql
{
    public const string Insert = """
    INSERT INTO analytics_access.user_option_scope_audit
    (
        option_id,
        previous_user_ids,
        new_user_ids,
        created_by,
        created_at
    )
    VALUES
    (
        @OptionId,
        @PreviousUserIdsJson,
        @NewUserIdsJson,
        @UserId,
        SYSUTCDATETIME()
    );
    """;
}
