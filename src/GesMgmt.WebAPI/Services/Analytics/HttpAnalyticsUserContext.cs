using System.Security.Claims;
using GesMgmt.Application.Interfaces.Analytics;

namespace GesMgmt.WebAPI.Services.Analytics
{
    internal sealed class HttpAnalyticsUserContext(
        IHttpContextAccessor httpContextAccessor) : IAnalyticsUserContext
    {
        private static readonly string[] UserIdClaimTypes =
        [
            "sisges_user_id",
            "user_id",
            ClaimTypes.NameIdentifier
        ];

        public bool TryGetUserId(out int userId)
        {
            var principal = httpContextAccessor.HttpContext?.User;

            if (principal is null)
            {
                userId = 0;
                return false;
            }

            foreach (var identity in principal.Identities.Where(x => x.IsAuthenticated))
            {
                foreach (var claimType in UserIdClaimTypes)
                {
                    var rawValue = identity.FindFirst(claimType)?.Value;

                    if (int.TryParse(rawValue, out userId) && userId > 0)
                    {
                        return true;
                    }
                }
            }

            userId = 0;
            return false;
        }
    }
}
