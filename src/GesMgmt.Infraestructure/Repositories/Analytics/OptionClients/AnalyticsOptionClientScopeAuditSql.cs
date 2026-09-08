using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence.Analytics;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

public static class AnalyticsOptionClientScopeAuditSql
{
    public const string Insert = """
    INSERT INTO analytics_access.option_client_scope_audit
    (
        option_id,
        previous_client_ids,
        new_client_ids,
        created_by,
        created_at
    )
    VALUES
    (
        @OptionId,
        @PreviousClientIdsJson,
        @NewClientIdsJson,
        @UserId,
        SYSUTCDATETIME()
    );
    """;
}
