using GesMgmt.Application.Utils.Analytics.PortfolioControlCenter;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace GesMgmt.Infraestructure.Persistence.Analytics
{
    internal sealed class AnalyticsDatabaseTelemetry : IDisposable
    {
        public const string MeterName = "Analytics.Api";
        public const string ActivitySourceName = "Analytics.Api";

        private readonly ActivitySource _activitySource =
            new(ActivitySourceName);
        private readonly Meter _meter = new(MeterName);
        private readonly Histogram<double> _operationDuration;
        private readonly Counter<long> _operations;
        private readonly Counter<long> _failures;

        public AnalyticsDatabaseTelemetry()
        {
            _operationDuration = _meter.CreateHistogram<double>(
                "analytics.api.database.operation.duration",
                unit: "ms",
                description: "Duración de operaciones de base de datos de Analytics.");
            _operations = _meter.CreateCounter<long>(
                "analytics.api.database.operations",
                unit: "operation",
                description: "Operaciones de base de datos de Analytics completadas.");
            _failures = _meter.CreateCounter<long>(
                "analytics.api.database.failures",
                unit: "operation",
                description: "Operaciones de base de datos de Analytics fallidas o canceladas.");
        }

        public async Task<T> ObserveAsync<T>(
            string databaseRole,
            string operation,
            Func<Task<T>> action)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(databaseRole);
            ArgumentException.ThrowIfNullOrWhiteSpace(operation);
            ArgumentNullException.ThrowIfNull(action);

            var startedAt = Stopwatch.GetTimestamp();
            var outcome = "success";

            using var activity = _activitySource.StartActivity(
                "analytics.database.operation",
                ActivityKind.Client);

            activity?.SetTag("db.system", "mssql");
            activity?.SetTag("analytics.db.role", databaseRole);
            activity?.SetTag("analytics.db.operation", operation);

            try
            {
                return await action();
            }
            catch (OperationCanceledException exception)
            {
                outcome = "cancelled";
                RecordFailure(activity, exception, outcome);
                throw;
            }
            catch (Exception exception)
            {
                outcome = "error";
                RecordFailure(activity, exception, outcome);
                throw;
            }
            finally
            {
                activity?.SetTag("analytics.db.outcome", outcome);
                var elapsed = Stopwatch.GetElapsedTime(startedAt);
                Record(databaseRole, operation, outcome, elapsed);
                PortfolioControlCenterDiagnostics.RecordDatabaseOperation(
                    databaseRole,
                    operation,
                    elapsed);
            }
        }

        public async Task ObserveAsync(
            string databaseRole,
            string operation,
            Func<Task> action)
        {
            await ObserveAsync(
                databaseRole,
                operation,
                async () =>
                {
                    await action();
                    return true;
                });
        }

        public void Dispose()
        {
            _activitySource.Dispose();
            _meter.Dispose();
        }

        private void Record(
            string databaseRole,
            string operation,
            string outcome,
            TimeSpan elapsed)
        {
            var tags = new TagList
            {
                { "db.system", "mssql" },
                { "analytics.db.role", databaseRole },
                { "analytics.db.operation", operation },
                { "analytics.db.outcome", outcome }
            };

            _operations.Add(1, tags);
            _operationDuration.Record(elapsed.TotalMilliseconds, tags);

            if (!string.Equals(outcome, "success", StringComparison.Ordinal))
            {
                _failures.Add(1, tags);
            }
        }

        private static void RecordFailure(
            Activity? activity,
            Exception exception,
            string outcome)
        {
            if (activity is null)
            {
                return;
            }

            activity.SetStatus(ActivityStatusCode.Error, outcome);
            activity.SetTag("error.type", exception.GetType().FullName);
        }
    }
}
