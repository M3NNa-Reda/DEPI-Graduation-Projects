namespace EasyShop.MVC.Services;

// Counts failed sign-ins per account (the IP rate limiter alone does not stop a botnet
// that spreads guesses for one email over many addresses). After too many failures the
// account is locked for a while. State is in memory, so it resets when the site restarts.
public sealed class LoginAttemptTracker
{
    public const int MaxFailures = 5;
    public static readonly TimeSpan LockDuration = TimeSpan.FromMinutes(15);

    private const int PruneThreshold = 5000;

    private readonly TimeProvider _clock;
    private readonly object _gate = new();
    private readonly Dictionary<string, Entry> _entries = new();

    public LoginAttemptTracker(TimeProvider clock)
    {
        _clock = clock;
    }

    public bool IsLocked(string key, out TimeSpan retryAfter)
    {
        lock (_gate)
        {
            var now = _clock.GetUtcNow();
            if (_entries.TryGetValue(Normalize(key), out var entry) && entry.LockedUntil > now)
            {
                retryAfter = entry.LockedUntil - now;
                return true;
            }
        }

        retryAfter = TimeSpan.Zero;
        return false;
    }

    public void RecordFailure(string key)
    {
        lock (_gate)
        {
            var now = _clock.GetUtcNow();
            PruneIfLarge(now);

            var normalized = Normalize(key);
            _entries.TryGetValue(normalized, out var entry);

            // Old failures (or a lock that already ended) do not count against the next attempt.
            int failures = entry == null || now - entry.LastFailure > LockDuration ? 0 : entry.Failures;
            failures++;

            _entries[normalized] = new Entry(
                failures,
                now,
                failures >= MaxFailures ? now + LockDuration : DateTimeOffset.MinValue);
        }
    }

    public void Reset(string key)
    {
        lock (_gate)
        {
            _entries.Remove(Normalize(key));
        }
    }

    private void PruneIfLarge(DateTimeOffset now)
    {
        if (_entries.Count < PruneThreshold)
        {
            return;
        }

        var stale = _entries
            .Where(pair => now - pair.Value.LastFailure > LockDuration)
            .Select(pair => pair.Key)
            .ToList();

        foreach (var key in stale)
        {
            _entries.Remove(key);
        }
    }

    private static string Normalize(string key) => key.Trim().ToLowerInvariant();

    private sealed record Entry(int Failures, DateTimeOffset LastFailure, DateTimeOffset LockedUntil);
}
