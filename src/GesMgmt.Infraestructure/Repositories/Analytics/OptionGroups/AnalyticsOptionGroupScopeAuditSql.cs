using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence.Analytics;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

public static class AnalyticsOptionGroupScopeAuditSql
{
    public const string Insert = """
        INSERT INTO analytics_access.option_group_scope_audit
        (
            option_id,
            previous_group_ids,
            new_group_ids,
            created_by,
            created_at
        )
        VALUES
        (
            @OptionId,
            @PreviousGroupIdsJson,
            @NewGroupIdsJson,
            @UserId,
            SYSUTCDATETIME()
        );
        """;
}
