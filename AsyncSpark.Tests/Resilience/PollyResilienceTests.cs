namespace AsyncSpark.Tests.Resilience;

/// <summary>
/// Tests demonstrating Polly resilience patterns with simulated transient failures
/// </summary>
[TestClass]
public class PollyResilienceTests
{
    private int _callCount = 0;

    /// <summary>
    /// Simulates a service that fails the first 2 times, then succeeds
    /// </summary>
    private async Task<string> TransientFailureService(CancellationToken ct = default)
    {
        _callCount++;
        await Task.Delay(100, ct); // Simulate network call

        if (_callCount < 3)
        {
            throw new HttpRequestException($"Transient failure #{_callCount}");
        }

        return "Success";
    }

    /// <summary>
    /// Simulates a service that always fails
    /// </summary>
    private async Task<string> PermanentFailureService(CancellationToken ct = default)
    {
        await Task.Delay(100, ct);
        throw new HttpRequestException("Permanent failure - service is down");
    }

    [TestInitialize]
    public void Setup()
    {
        _callCount = 0;
    }

    /// <summary>
    /// Tests that retry policy succeeds after transient failures
    /// </summary>
    /// <remarks>
    /// **Pattern**: Polly retry with exponential backoff
    ///
    /// **What this shows**: How retries can overcome transient failures (network blips, temporary 500 errors)
    ///
    /// **Real-world scenario**: API temporarily returns 503, retrying succeeds
    /// </remarks>
    [TestMethod]
    public async Task RetryPolicy_SucceedsAfterTransientFailures()
    {
        // Arrange
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt) * 100),
                onRetry: (exception, timespan, retryCount, context) =>
                {
                    Console.WriteLine($"Retry {retryCount} after {timespan.TotalMilliseconds}ms due to: {exception.Message}");
                });

        // Act
        var result = await retryPolicy.ExecuteAsync(async () => await TransientFailureService());

        // Assert
        Assert.AreEqual("Success", result);
        Assert.AreEqual(3, _callCount, "Should have tried 3 times (2 failures + 1 success)");
    }

    /// <summary>
    /// Tests that retry policy eventually gives up on permanent failures
    /// </summary>
    /// <remarks>
    /// **Pattern**: Retry exhaustion
    ///
    /// **What this shows**: Retries should eventually give up to avoid wasting resources
    ///
    /// **Real-world scenario**: Service is completely down, retrying won't help
    /// </remarks>
    [TestMethod]
    public async Task RetryPolicy_GivesUpOnPermanentFailure()
    {
        // Arrange
        int retryCount = 0;
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromMilliseconds(50),
                onRetry: (exception, timespan, retry, context) =>
                {
                    retryCount = retry;
                    Console.WriteLine($"Retry {retry} failed: {exception.Message}");
                });

        // Act & Assert
        bool exceptionThrown = false;
        try
        {
            await retryPolicy.ExecuteAsync(async () => await PermanentFailureService());
        }
        catch (HttpRequestException)
        {
            exceptionThrown = true;
        }

        Assert.IsTrue(exceptionThrown, "Should have thrown HttpRequestException");
        Assert.AreEqual(3, retryCount, "Should have retried 3 times before giving up");
    }

    /// <summary>
    /// Tests retry with cancellation - operation should stop when cancelled
    /// </summary>
    /// <remarks>
    /// **Pattern**: Retry with cancellation
    ///
    /// **What this shows**: Even with retries, cancellation should immediately stop the operation
    ///
    /// **Important**: Always pass cancellation tokens through retry policies
    /// </remarks>
    [TestMethod]
    public async Task RetryPolicy_RespectsCancellation()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(
                retryCount: 5,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(1));

        // Act & Assert
        cts.CancelAfter(150); // Cancel after first attempt

        bool exceptionThrown = false;
        try
        {
            await retryPolicy.ExecuteAsync(async (ct) =>
            {
                await Task.Delay(100, ct);
                throw new HttpRequestException("Simulated failure");
            }, cts.Token);
        }
        catch (OperationCanceledException)
        {
            exceptionThrown = true;
        }

        Assert.IsTrue(exceptionThrown, "Should have thrown OperationCanceledException");
    }

    /// <summary>
    /// Tests timeout policy - operation should fail if it takes too long
    /// </summary>
    /// <remarks>
    /// **Pattern**: Polly timeout policy
    ///
    /// **What this shows**: How to protect against slow operations with timeouts
    ///
    /// **Real-world scenario**: API is responding but very slowly, timeout prevents tying up resources
    /// </remarks>
    [TestMethod]
    public async Task TimeoutPolicy_FailsSlowOperations()
    {
        // Arrange
        var timeoutPolicy = Policy.TimeoutAsync(TimeSpan.FromMilliseconds(200));

        // Act & Assert - Operation takes 500ms, timeout is 200ms
        bool exceptionThrown = false;
        try
        {
            await timeoutPolicy.ExecuteAsync(async () =>
            {
                await Task.Delay(500); // Slow operation
                return "Should not complete";
            });
        }
        catch (TimeoutRejectedException)
        {
            exceptionThrown = true;
        }

        Assert.IsTrue(exceptionThrown, "Should have thrown TimeoutRejectedException");
    }

    /// <summary>
    /// Tests combining retry and timeout policies
    /// </summary>
    /// <remarks>
    /// **Pattern**: Policy wrap (combining multiple policies)
    ///
    /// **What this shows**: How to combine retry and timeout for robust error handling
    ///
    /// **Real-world scenario**: Retry transient failures, but timeout each attempt
    /// </remarks>
    [TestMethod]
    public async Task CombinedPolicy_RetryWithTimeout()
    {
        // Arrange
        int attemptCount = 0;
        var timeoutPolicy = Policy.TimeoutAsync(TimeSpan.FromMilliseconds(300));
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 2,
                sleepDurationProvider: retryAttempt => TimeSpan.FromMilliseconds(100),
                onRetry: (exception, timespan, retry, context) =>
                {
                    Console.WriteLine($"Retry {retry} after: {exception.Message}");
                });

        // Wrap timeout inside retry
        var combinedPolicy = Policy.WrapAsync(retryPolicy, timeoutPolicy);

        // Act - Fast enough to succeed on retry
        var result = await combinedPolicy.ExecuteAsync(async () =>
        {
            attemptCount++;
            await Task.Delay(100);

            if (attemptCount < 2)
            {
                throw new HttpRequestException($"Transient failure {attemptCount}");
            }

            return "Success after retry";
        });

        // Assert
        Assert.AreEqual("Success after retry", result);
        Assert.AreEqual(2, attemptCount);
    }

    /// <summary>
    /// Tests jitter in retry delays to prevent thundering herd
    /// </summary>
    /// <remarks>
    /// **Pattern**: Retry with jitter
    ///
    /// **What this shows**: Adding randomness to retry delays prevents many clients from retrying simultaneously
    ///
    /// **Real-world scenario**: Service comes back online, want to avoid retry storm
    /// </remarks>
    [TestMethod]
    public async Task RetryPolicy_UsesJitterToAvoidThunderingHerd()
    {
        // Arrange
        var random = new Random();
        var delays = new List<TimeSpan>();

        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt =>
                {
                    // Exponential backoff + jitter
                    var baseDelay = TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt) * 100);
                    var jitter = TimeSpan.FromMilliseconds(random.Next(0, 100));
                    var totalDelay = baseDelay + jitter;
                    delays.Add(totalDelay);
                    return totalDelay;
                });

        // Act
        try
        {
            await retryPolicy.ExecuteAsync(async () =>
            {
                await Task.Delay(10);
                throw new HttpRequestException("Keep failing");
            });
        }
        catch
        {
            // Expected to fail
        }

        // Assert - Delays should have jitter (not be exactly powers of 2)
        Assert.AreEqual(3, delays.Count);
        Assert.IsTrue(delays[0].TotalMilliseconds >= 200); // 2^1 * 100 + jitter
        Assert.IsTrue(delays[1].TotalMilliseconds >= 400); // 2^2 * 100 + jitter
        Assert.IsTrue(delays[2].TotalMilliseconds >= 800); // 2^3 * 100 + jitter

        // Each delay should be slightly different due to jitter
        Console.WriteLine($"Delays with jitter: {string.Join(", ", delays.Select(d => $"{d.TotalMilliseconds}ms"))}");
    }

    /// <summary>
    /// Tests selective retry - only retry on specific exceptions
    /// </summary>
    /// <remarks>
    /// **Pattern**: Selective retry
    ///
    /// **What this shows**: Not all errors should be retried (e.g., don't retry validation errors)
    ///
    /// **Real-world scenario**: Retry 500 errors but not 400 errors
    /// </remarks>
    [TestMethod]
    public async Task RetryPolicy_OnlyRetriesSpecificExceptions()
    {
        // Arrange
        int attemptCount = 0;
        var retryPolicy = Policy
            .Handle<HttpRequestException>() // Only retry HttpRequestException
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromMilliseconds(50));

        // Act & Assert - Throw ArgumentException (not retried)
        bool exceptionThrown = false;
        try
        {
            await retryPolicy.ExecuteAsync(async () =>
            {
                attemptCount++;
                await Task.Delay(10);
                throw new ArgumentException("Validation error - should not retry");
            });
        }
        catch (ArgumentException)
        {
            exceptionThrown = true;
        }

        Assert.IsTrue(exceptionThrown, "Should have thrown ArgumentException");
        Assert.AreEqual(1, attemptCount, "Should not retry ArgumentException, only tried once");
    }
}
