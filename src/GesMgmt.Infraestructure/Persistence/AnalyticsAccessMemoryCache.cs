using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using GesMgmt.Application.Interfaces.Analytics;

namespace GesMgmt.Infraestructure.Persistence;

public sealed class AnalyticsAccessMemoryCache(IMemoryCache cache)
    : IAnalyticsAccessCache
{
    private readonly ConcurrentDictionary<string, CacheKeyState> _keyStates =
        new(StringComparer.Ordinal);
    private readonly object _invalidationSync = new();
    private long _invalidationVersion;

    public int ActiveKeyStateCount => _keyStates.Count;

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        TimeSpan duration,
        Func<CancellationToken, Task<T>> factory,
        CancellationToken cancellationToken)
    {
        if (cache.TryGetValue<T>(key, out var cached))
        {
            return cached!;
        }

        var state = AcquireState(key);

        try
        {
            await state.Gate.WaitAsync(cancellationToken);

            try
            {
                if (cache.TryGetValue<T>(key, out cached))
                {
                    return cached!;
                }

                // A single generation prevents stale publication after any
                // administrative invalidation without retaining per-key versions.
                var version = Volatile.Read(ref _invalidationVersion);
                var value = await factory(cancellationToken);

                lock (_invalidationSync)
                {
                    if (_invalidationVersion == version)
                    {
                        cache.Set(key, value, duration);
                    }
                }

                return value;
            }
            finally
            {
                state.Gate.Release();
            }
        }
        finally
        {
            ReleaseState(key, state);
        }
    }

    public void Remove(string key)
    {
        lock (_invalidationSync)
        {
            _invalidationVersion = unchecked(_invalidationVersion + 1);
            cache.Remove(key);
        }
    }

    private CacheKeyState AcquireState(string key)
    {
        while (true)
        {
            var state = _keyStates.GetOrAdd(
                key,
                static _ => new CacheKeyState());

            if (state.TryAcquire())
            {
                return state;
            }

            _keyStates.TryRemove(
                new KeyValuePair<string, CacheKeyState>(key, state));
        }
    }

    private void ReleaseState(string key, CacheKeyState state)
    {
        if (!state.Release())
        {
            return;
        }

        _keyStates.TryRemove(
            new KeyValuePair<string, CacheKeyState>(key, state));
        state.Dispose();
    }

    private sealed class CacheKeyState : IDisposable
    {
        private readonly object _sync = new();
        private int _references;
        private bool _retired;

        public SemaphoreSlim Gate { get; } = new(1, 1);

        public bool TryAcquire()
        {
            lock (_sync)
            {
                if (_retired)
                {
                    return false;
                }

                _references++;
                return true;
            }
        }

        public bool Release()
        {
            lock (_sync)
            {
                _references--;
                if (_references != 0)
                {
                    return false;
                }

                _retired = true;
                return true;
            }
        }

        public void Dispose()
        {
            Gate.Dispose();
        }
    }
}
