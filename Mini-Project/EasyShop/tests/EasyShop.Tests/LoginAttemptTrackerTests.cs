using EasyShop.MVC.Services;

namespace EasyShop.Tests;

public class LoginAttemptTrackerTests
{
    private sealed class ManualClock : TimeProvider
    {
        public DateTimeOffset Now { get; set; } = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

        public override DateTimeOffset GetUtcNow() => Now;
    }

    private readonly ManualClock _clock = new();
    private readonly LoginAttemptTracker _tracker;

    public LoginAttemptTrackerTests()
    {
        _tracker = new LoginAttemptTracker(_clock);
    }

    private void Fail(string email, int times)
    {
        for (int i = 0; i < times; i++)
        {
            _tracker.RecordFailure(email);
        }
    }

    [Fact]
    public void An_account_is_not_locked_before_the_limit()
    {
        Fail("a@b.com", LoginAttemptTracker.MaxFailures - 1);

        Assert.False(_tracker.IsLocked("a@b.com", out _));
    }

    [Fact]
    public void An_account_is_locked_after_too_many_failures()
    {
        Fail("a@b.com", LoginAttemptTracker.MaxFailures);

        Assert.True(_tracker.IsLocked("a@b.com", out var retryAfter));
        Assert.Equal(LoginAttemptTracker.LockDuration, retryAfter);
    }

    [Fact]
    public void The_lock_ends_after_the_lock_duration()
    {
        Fail("a@b.com", LoginAttemptTracker.MaxFailures);

        _clock.Now += LoginAttemptTracker.LockDuration + TimeSpan.FromSeconds(1);

        Assert.False(_tracker.IsLocked("a@b.com", out _));
    }

    [Fact]
    public void Emails_are_compared_without_case_or_spaces()
    {
        Fail("A@B.com", LoginAttemptTracker.MaxFailures);

        Assert.True(_tracker.IsLocked(" a@b.com ", out _));
    }

    [Fact]
    public void One_account_being_locked_does_not_affect_another()
    {
        Fail("a@b.com", LoginAttemptTracker.MaxFailures);

        Assert.False(_tracker.IsLocked("other@b.com", out _));
    }

    [Fact]
    public void A_successful_sign_in_clears_the_failures()
    {
        Fail("a@b.com", LoginAttemptTracker.MaxFailures - 1);
        _tracker.Reset("a@b.com");
        Fail("a@b.com", LoginAttemptTracker.MaxFailures - 1);

        Assert.False(_tracker.IsLocked("a@b.com", out _));
    }

    [Fact]
    public void Old_failures_stop_counting()
    {
        Fail("a@b.com", LoginAttemptTracker.MaxFailures - 1);
        _clock.Now += LoginAttemptTracker.LockDuration + TimeSpan.FromMinutes(1);
        Fail("a@b.com", 1);

        Assert.False(_tracker.IsLocked("a@b.com", out _));
    }
}
