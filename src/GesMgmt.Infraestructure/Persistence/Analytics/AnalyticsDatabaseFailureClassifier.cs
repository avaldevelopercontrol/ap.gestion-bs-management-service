using System.Net.Sockets;
using System.Security.Authentication;
using Microsoft.Data.SqlClient;

namespace GesMgmt.Infraestructure.Persistence.Analytics
{
    public static class AnalyticsDatabaseFailureClassifier
    {
        private static readonly string[] TlsMarkers =
        [
            "pre-login handshake",
            "prelogin handshake",
            "ssl provider",
            "tls handshake",
            "ssl/tls",
            "unexpected eof"
        ];

        private static readonly string[] AuthenticationMarkers =
        [
            "login failed for user",
            "authentication failed"
        ];

        private static readonly string[] ConnectivityMarkers =
        [
            "server was not found",
            "server is not accessible",
            "network-related",
            "instance-specific",
            "actively refused",
            "connection timeout",
            "connection timed out"
        ];

        public static AnalyticsDatabaseFailureCategory Classify(
            Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);

            foreach (var current in EnumerateExceptionChain(exception))
            {
                if (IsConfigurationFailure(current))
                {
                    return AnalyticsDatabaseFailureCategory.Configuration;
                }

                if (IsTlsFailure(current))
                {
                    return AnalyticsDatabaseFailureCategory.TlsHandshake;
                }

                if (IsAuthenticationFailure(current))
                {
                    return AnalyticsDatabaseFailureCategory.Authentication;
                }

                if (IsConnectivityFailure(current))
                {
                    return AnalyticsDatabaseFailureCategory.Connectivity;
                }
            }

            return AnalyticsDatabaseFailureCategory.Unknown;
        }

        private static IEnumerable<Exception> EnumerateExceptionChain(
            Exception exception)
        {
            for (Exception? current = exception;
                 current is not null;
                 current = current.InnerException)
            {
                yield return current;
            }
        }

        private static bool IsConfigurationFailure(Exception exception) =>
            exception is InvalidOperationException &&
            ContainsAny(
                exception.Message,
                "ConnectionStrings:Analytics",
                "ConnectionStrings:AvalCobConnection");

        private static bool IsTlsFailure(Exception exception) =>
            exception is AuthenticationException ||
            ContainsAny(exception.Message, TlsMarkers);

        private static bool IsAuthenticationFailure(Exception exception) =>
            (exception is SqlException sqlException &&
             sqlException.Number == 18456) ||
            ContainsAny(exception.Message, AuthenticationMarkers);

        private static bool IsConnectivityFailure(Exception exception) =>
            exception is SocketException or TimeoutException ||
            ContainsAny(exception.Message, ConnectivityMarkers);

        private static bool ContainsAny(
            string value,
            params string[] markers) =>
            markers.Any(marker =>
                value.Contains(marker, StringComparison.OrdinalIgnoreCase));
    }
}
