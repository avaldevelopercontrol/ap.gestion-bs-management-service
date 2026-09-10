using System.Diagnostics;

namespace GesMgmt.Application.Utils.Analytics.PortfolioControlCenter;

public static class PortfolioControlCenterDiagnostics
{
    public const string AccessPhase = "access";
    public const string ContextPhase = "context";
    public const string QueryPhase = "query";

    private static readonly AsyncLocal<RequestState?> CurrentRequest = new();
    private static readonly AsyncLocal<string?> CurrentPhase = new();

    public static IDisposable BeginRequest(string endpoint)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(endpoint);

        var previousRequest = CurrentRequest.Value;
        var previousPhase = CurrentPhase.Value;
        CurrentRequest.Value = new RequestState(endpoint);
        CurrentPhase.Value = null;

        return new DelegateScope(() =>
        {
            CurrentRequest.Value = previousRequest;
            CurrentPhase.Value = previousPhase;
        });
    }

    public static async Task<T> ObservePhaseAsync<T>(
        string phase,
        Func<Task<T>> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        using var phaseScope = BeginPhase(phase);
        return await action();
    }

    public static IDisposable BeginPhase(string phase)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(phase);

        var request = CurrentRequest.Value;
        if (request is null)
        {
            return DelegateScope.Empty;
        }

        var previousPhase = CurrentPhase.Value;
        CurrentPhase.Value = phase;
        var startedAt = Stopwatch.GetTimestamp();

        return new DelegateScope(() =>
        {
            request.RecordPhase(
                phase,
                Stopwatch.GetElapsedTime(startedAt));
            CurrentPhase.Value = previousPhase;
        });
    }

    public static void RecordDatabaseOperation(
        string databaseRole,
        string operation,
        TimeSpan elapsed)
    {
        var request = CurrentRequest.Value;
        if (request is null)
        {
            return;
        }

        request.RecordDatabaseOperation(
            databaseRole,
            operation,
            CurrentPhase.Value,
            elapsed);
    }

    public static PortfolioControlCenterRequestSnapshot Snapshot()
    {
        var request = CurrentRequest.Value;
        return request?.CreateSnapshot()
            ?? PortfolioControlCenterRequestSnapshot.Empty;
    }

    public static PortfolioControlCenterOperationContext? CurrentOperationContext()
    {
        var request = CurrentRequest.Value;
        if (request is null)
        {
            return null;
        }

        var phase = CurrentPhase.Value;
        return new PortfolioControlCenterOperationContext(
            request.Endpoint,
            string.IsNullOrWhiteSpace(phase)
                ? "unclassified"
                : phase!);
    }

    private sealed class RequestState(string endpoint)
    {
        public string Endpoint => endpoint;

        private readonly object _gate = new();
        private readonly Dictionary<string, double> _phaseMilliseconds =
            new(StringComparer.Ordinal);
        private readonly Dictionary<string, int> _databaseCommandsByRole =
            new(StringComparer.Ordinal);
        private readonly Dictionary<string, int> _databaseCommandsByPhase =
            new(StringComparer.Ordinal);
        private double _databaseCommandMilliseconds;
        private int _databaseCommandCount;
        private int _connectionOpenCount;

        public void RecordPhase(string phase, TimeSpan elapsed)
        {
            lock (_gate)
            {
                _phaseMilliseconds.TryGetValue(phase, out var current);
                _phaseMilliseconds[phase] = current + elapsed.TotalMilliseconds;
            }
        }

        public void RecordDatabaseOperation(
            string databaseRole,
            string operation,
            string? phase,
            TimeSpan elapsed)
        {
            lock (_gate)
            {
                if (string.Equals(
                        operation,
                        "connection.open",
                        StringComparison.Ordinal))
                {
                    _connectionOpenCount++;
                    return;
                }

                _databaseCommandCount++;
                _databaseCommandMilliseconds += elapsed.TotalMilliseconds;

                _databaseCommandsByRole.TryGetValue(databaseRole, out var roleCount);
                _databaseCommandsByRole[databaseRole] = roleCount + 1;

                var normalizedPhase = string.IsNullOrWhiteSpace(phase)
                    ? "unclassified"
                    : phase;
                _databaseCommandsByPhase.TryGetValue(normalizedPhase, out var phaseCount);
                _databaseCommandsByPhase[normalizedPhase] = phaseCount + 1;
            }
        }

        public PortfolioControlCenterRequestSnapshot CreateSnapshot()
        {
            lock (_gate)
            {
                return new PortfolioControlCenterRequestSnapshot(
                    endpoint,
                    GetPhaseMilliseconds(AccessPhase),
                    GetPhaseMilliseconds(ContextPhase),
                    GetPhaseMilliseconds(QueryPhase),
                    _databaseCommandCount,
                    _connectionOpenCount,
                    _databaseCommandMilliseconds,
                    GetCount(_databaseCommandsByRole, "analytics"),
                    GetCount(_databaseCommandsByRole, "sisges"),
                    GetCount(_databaseCommandsByPhase, AccessPhase),
                    GetCount(_databaseCommandsByPhase, ContextPhase),
                    GetCount(_databaseCommandsByPhase, QueryPhase),
                    GetCount(_databaseCommandsByPhase, "unclassified"));
            }
        }

        private double GetPhaseMilliseconds(string phase) =>
            _phaseMilliseconds.TryGetValue(phase, out var value)
                ? value
                : 0d;

        private static int GetCount(
            IReadOnlyDictionary<string, int> source,
            string key) =>
            source.TryGetValue(key, out var value)
                ? value
                : 0;
    }

    private sealed class DelegateScope(Action onDispose) : IDisposable
    {
        public static readonly DelegateScope Empty = new(static () => { });

        private Action? _onDispose = onDispose;

        public void Dispose()
        {
            Interlocked.Exchange(ref _onDispose, null)?.Invoke();
        }
    }
}

public sealed record PortfolioControlCenterRequestSnapshot(
    string Endpoint,
    double AccessMilliseconds,
    double ContextMilliseconds,
    double QueryMilliseconds,
    int DatabaseCommandCount,
    int ConnectionOpenCount,
    double DatabaseCommandMilliseconds,
    int AnalyticsDatabaseCommands,
    int SisgesDatabaseCommands,
    int AccessDatabaseCommands,
    int ContextDatabaseCommands,
    int QueryDatabaseCommands,
    int UnclassifiedDatabaseCommands)
{
    public static readonly PortfolioControlCenterRequestSnapshot Empty = new(
        "unknown",
        0d,
        0d,
        0d,
        0,
        0,
        0d,
        0,
        0,
        0,
        0,
        0,
        0);
}

public sealed record PortfolioControlCenterOperationContext(
    string Endpoint,
    string Phase);
