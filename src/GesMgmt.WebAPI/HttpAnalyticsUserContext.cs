using System.Security.Claims;
using GesMgmt.Application.Interfaces.Analytics;

namespace GesMgmt.WebAPI
{
    internal sealed class HttpAnalyticsUserContext(
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration,
        IHostEnvironment hostEnvironment) : IAnalyticsUserContext
    {
        private const string LocalTestingUserIdKey = "AnalyticsTesting:UserId";
        private const string LocalTestingGroupIdKey = "AnalyticsTesting:GroupId";
        private const string DevelopmentUserIdHeader = "X-Sisges-User-Id";
        private const string DevelopmentGroupIdHeader = "X-Sisges-Group-Id";

        private static readonly string[] UserIdClaimTypes =
        [
            "sisges_user_id",
            "user_id",
            ClaimTypes.NameIdentifier
        ];

        private static readonly string[] GroupIdClaimTypes =
        [
            "sisges_group_id",
            "group_id"
        ];

        public bool TryGetUserId(out int userId) =>
            TryGetPositiveIdentifier(
                UserIdClaimTypes,
                DevelopmentUserIdHeader,
                LocalTestingUserIdKey,
                out userId);

        public bool TryGetGroupId(out int groupId) =>
            TryGetPositiveIdentifier(
                GroupIdClaimTypes,
                DevelopmentGroupIdHeader,
                LocalTestingGroupIdKey,
                out groupId);

        private bool TryGetPositiveIdentifier(
            IReadOnlyCollection<string> claimTypes,
            string developmentHeader,
            string localTestingKey,
            out int identifier)
        {
            var principal = httpContextAccessor.HttpContext?.User;

            if (principal is not null)
            {
                foreach (var identity in principal.Identities.Where(x => x.IsAuthenticated))
                {
                    foreach (var claimType in claimTypes)
                    {
                        var rawValue = identity.FindFirst(claimType)?.Value;

                        if (int.TryParse(rawValue, out identifier) && identifier > 0)
                        {
                            return true;
                        }
                    }
                }
            }

            // Sólo en Development se acepta el contexto SISGES propagado por el
            // frontend legacy. En otros ambientes debe provenir del host autenticado.
            if (hostEnvironment.IsDevelopment())
            {
                var headerValue = httpContextAccessor.HttpContext?
                    .Request.Headers[developmentHeader]
                    .FirstOrDefault();

                if (int.TryParse(headerValue, out identifier) && identifier > 0)
                {
                    return true;
                }

                if (int.TryParse(configuration[localTestingKey], out identifier) &&
                    identifier > 0)
                {
                    return true;
                }
            }

            identifier = 0;
            return false;
        }
    }
}
