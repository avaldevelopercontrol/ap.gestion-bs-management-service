using System.Security.Claims;
using GesMgmt.Application.Interfaces.Analytics;

namespace GesMgmt.WebAPI.Services.Analytics
{
    internal sealed class HttpAnalyticsUserContext(
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration,
        IHostEnvironment hostEnvironment) : IAnalyticsUserContext
    {
        private const string LocalTestingUserIdKey = "AnalyticsTesting:UserId";

        private static readonly string[] UserIdClaimTypes =
        [
            "sisges_user_id",
            "user_id",
            ClaimTypes.NameIdentifier
        ];

        public bool TryGetUserId(out int userId)
        {
            var principal = httpContextAccessor.HttpContext?.User;

            if (principal is not null)
            {
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
            }

            // Soporte exclusivo para smoke tests locales del frontend.
            // No acepta headers/query params y nunca se habilita fuera de Development.
            if (hostEnvironment.IsDevelopment() &&
                int.TryParse(configuration[LocalTestingUserIdKey], out userId) &&
                userId > 0)
            {
                return true;
            }

            userId = 0;
            return false;
        }
    }
}
