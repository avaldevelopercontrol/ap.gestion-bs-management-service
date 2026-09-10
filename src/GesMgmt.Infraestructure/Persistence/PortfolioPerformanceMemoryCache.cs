using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;

namespace GesMgmt.Infraestructure.Persistence;

internal sealed class PortfolioPerformanceMemoryCache :
    IPortfolioPerformanceCache,
    IDisposable
{
    private readonly MemoryCache _cache;
    private readonly ConcurrentDictionary<string, PendingWork> _inflight =
        new(StringComparer.Ordinal);

    public PortfolioPerformanceMemoryCache(
        PortfolioControlCenterPerformanceOptions performance)
    {
        ArgumentNullException.ThrowIfNull(performance);

        _cache = new MemoryCache(
            new MemoryCacheOptions
            {
                SizeLimit = performance.DetailCacheMaxEntries
            });
    }

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        TimeSpan duration,
        Func<CancellationToken, Task<T>> factory,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(factory);

        if (duration <= TimeSpan.Zero)
        {
            return await factory(cancellationToken);
        }

        if (_cache.TryGetValue<CacheEntry<T>>(key, out var cached))
        {
            return cached!.Value;
        }

        while (true)
        {
            var pending = _inflight.GetOrAdd(
                key,
                _ => CreatePending<T>(key, duration, factory));

            if (pending.ResultType != typeof(T))
            {
                throw new InvalidOperationException(
                    $"La clave de cache '{key}' fue reutilizada con un tipo incompatible.");
            }

            if (pending.IsCancellationRequested)
            {
                _inflight.TryRemove(
                    new KeyValuePair<string, PendingWork>(key, pending));
                continue;
            }

            object? result;
            try
            {
                result = await pending.WaitAsync(cancellationToken);
            }
            catch (OperationCanceledException)
                when (!cancellationToken.IsCancellationRequested &&
                      pending.IsCancellationRequested)
            {
                // The previous waiter abandoned this single-flight just before
                // this caller joined it. Retry with a fresh producer instead of
                // surfacing another request's cancellation.
                _inflight.TryRemove(
                    new KeyValuePair<string, PendingWork>(key, pending));
                continue;
            }

            if (result is not CacheEntry<T> typed)
            {
                throw new InvalidOperationException(
                    $"La clave de cache '{key}' devolvió un tipo incompatible.");
            }

            return typed.Value;
        }
    }

    public void Dispose()
    {
        foreach (var pending in _inflight.Values)
        {
            pending.Cancel();
        }

        _cache.Dispose();
    }

    private PendingWork CreatePending<T>(
        string key,
        TimeSpan duration,
        Func<CancellationToken, Task<T>> factory)
    {
        PendingWork? pending = null;

        pending = new PendingWork(
            typeof(T),
            async workCancellationToken =>
            {
                try
                {
                    if (_cache.TryGetValue<CacheEntry<T>>(key, out var cached))
                    {
                        return cached;
                    }

                    var value = await factory(workCancellationToken);

                    // A factory may ignore CancellationToken. If every waiter has
                    // already gone away, do not publish an abandoned result.
                    workCancellationToken.ThrowIfCancellationRequested();

                    var entry = new CacheEntry<T>(value);
                    _cache.Set(
                        key,
                        entry,
                        new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = duration,
                            Size = 1
                        });
                    return entry;
                }
                finally
                {
                    _inflight.TryRemove(
                        new KeyValuePair<string, PendingWork>(
                            key,
                            pending!));
                }
            });

        return pending;
    }

    private sealed class PendingWork
    {
        private readonly CancellationTokenSource _workCancellation = new();
        private readonly Lazy<Task<object?>> _task;
        private int _waiterCount;

        public PendingWork(
            Type resultType,
            Func<CancellationToken, Task<object?>> producer)
        {
            ResultType = resultType;
            _task = new Lazy<Task<object?>>(
                () => producer(_workCancellation.Token),
                LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public Type ResultType { get; }

        public bool IsCancellationRequested =>
            _workCancellation.IsCancellationRequested;

        public async Task<object?> WaitAsync(
            CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _waiterCount);
            var task = _task.Value;

            try
            {
                return await task.WaitAsync(cancellationToken);
            }
            finally
            {
                if (Interlocked.Decrement(ref _waiterCount) == 0 &&
                    !task.IsCompleted)
                {
                    Cancel();
                }
            }
        }

        public void Cancel()
        {
            try
            {
                _workCancellation.Cancel();
            }
            catch (ObjectDisposedException)
            {
                // Defensive only; PendingWork currently owns the CTS for its
                // whole lifetime, but cancellation must remain idempotent.
            }
        }
    }

    private sealed record CacheEntry<T>(T Value);
}
