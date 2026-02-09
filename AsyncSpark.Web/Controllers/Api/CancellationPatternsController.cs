namespace AsyncSpark.Web.Controllers.Api;

/// <summary>
/// API Controller demonstrating cancellation token patterns
/// </summary>
[ApiController]
[Route("api/cancellation")]
[Tags("2. Cancellation Patterns")]
public class CancellationPatternsController : ControllerBase
{
    private readonly ILogger<CancellationPatternsController> _logger;
    private readonly AsyncMockService _mockService;

    /// <summary>
    /// Initializes a new instance of the CancellationPatternsController
    /// </summary>
    /// <param name="logger">Logger for diagnostic output</param>
    public CancellationPatternsController(ILogger<CancellationPatternsController> logger)
    {
        _logger = logger;
        _mockService = new AsyncMockService();
    }

    /// <summary>
    /// Long-running operation WITHOUT cancellation support
    /// </summary>
    /// <param name="iterations">Number of iterations (default: 100)</param>
    /// <returns>Result after completing all iterations</returns>
    /// <remarks>
    /// **Pattern**: Async without cancellation token (anti-pattern)
    ///
    /// **What this shows**: A long-running operation that CANNOT be cancelled.
    ///
    /// **What can go wrong**:
    /// - If the user navigates away, the server keeps working
    /// - Wastes CPU, memory, and connection pool resources
    /// - In production, this causes scalability issues
    ///
    /// **Try it**:
    /// 1. Call with iterations=1000
    /// 2. Cancel the request in your browser/tool
    /// 3. Check server logs - it keeps running!
    ///
    /// **Compare with**: `/api/cancellation/with-token` for the correct approach.
    /// </remarks>
    [HttpGet("no-cancellation")]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<IActionResult> NoCancellation([FromQuery] int iterations = 100)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        _logger.LogInformation("Starting NoCancellation with {Iterations} iterations - CANNOT be cancelled", iterations);

        try
        {
            var result = await _mockService.LongRunningOperation(iterations);
            stopwatch.Stop();

            _logger.LogInformation("NoCancellation completed after {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);

            return Ok(new
            {
                result,
                iterations,
                elapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                cancellable = false,
                warning = "This operation could not be cancelled even if you tried!"
            });
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error in NoCancellation");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Long-running operation WITH cancellation support
    /// </summary>
    /// <param name="iterations">Number of iterations (default: 100)</param>
    /// <param name="cancellationToken">Cancellation token from HTTP request</param>
    /// <returns>Result or cancellation response</returns>
    /// <remarks>
    /// **Pattern**: Proper cancellation token usage
    ///
    /// **What this shows**: How to wire cancellation from HTTP request through to your async operation.
    ///
    /// **Key techniques**:
    /// - Accept `CancellationToken` parameter (ASP.NET Core auto-wires it from HTTP request)
    /// - Pass it through entire call chain
    /// - Check `token.IsCancellationRequested` in long loops
    /// - Return 499 "Client Closed Request" status code
    ///
    /// **Try it**:
    /// 1. Call with iterations=1000
    /// 2. Cancel the request quickly
    /// 3. Check logs - operation stops immediately!
    ///
    /// **Real-world impact**: Proper cancellation can reduce server load by 30%+ in user-facing APIs.
    /// </remarks>
    [HttpGet("with-token")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(499)]
    public async Task<IActionResult> WithCancellationToken(
        [FromQuery] int iterations = 100,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        _logger.LogInformation("Starting WithCancellationToken with {Iterations} iterations - cancellable", iterations);

        try
        {
            var result = await _mockService.LongRunningOperationWithCancellationTokenAsync(iterations, cancellationToken);
            stopwatch.Stop();

            _logger.LogInformation("WithCancellationToken completed after {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);

            return Ok(new
            {
                result,
                iterations,
                elapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                cancellable = true,
                message = "This operation properly responds to cancellation requests"
            });
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();
            _logger.LogWarning("WithCancellationToken was cancelled after {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);

            return StatusCode(499, new
            {
                message = "Operation cancelled by client",
                iterationsRequested = iterations,
                elapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                cancellable = true
            });
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error in WithCancellationToken");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Demonstrates combining timeout and HTTP cancellation tokens
    /// </summary>
    /// <param name="iterations">Number of iterations</param>
    /// <param name="timeoutSeconds">Timeout in seconds (default: 5)</param>
    /// <param name="cancellationToken">HTTP request cancellation token</param>
    /// <returns>Result or appropriate error</returns>
    /// <remarks>
    /// **Pattern**: Linked cancellation tokens
    ///
    /// **What this shows**: How to combine multiple cancellation sources:
    /// 1. HTTP request cancellation (user navigates away)
    /// 2. Timeout cancellation (operation takes too long)
    ///
    /// **Key API**: `CancellationTokenSource.CreateLinkedTokenSource()`
    ///
    /// **Behavior**:
    /// - If EITHER source triggers, operation cancels
    /// - We can distinguish which one triggered by checking individual tokens
    /// - Returns different status codes: 408 for timeout, 499 for user cancellation
    ///
    /// **Try it**:
    /// - Call with iterations=200, timeoutSeconds=1 to trigger timeout
    /// - Call with iterations=50, then cancel request to trigger HTTP cancellation
    /// </remarks>
    [HttpGet("with-timeout")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(408)]
    [ProducesResponseType(499)]
    public async Task<IActionResult> WithTimeout(
        [FromQuery] int iterations = 100,
        [FromQuery] int timeoutSeconds = 5,
        CancellationToken cancellationToken = default)
    {
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        _logger.LogInformation(
            "Starting WithTimeout with {Iterations} iterations and {TimeoutSeconds}s timeout",
            iterations, timeoutSeconds);

        try
        {
            var result = await _mockService.LongRunningOperationWithCancellationTokenAsync(
                iterations,
                linkedCts.Token);

            stopwatch.Stop();

            return Ok(new
            {
                result,
                iterations,
                timeoutSeconds,
                elapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                message = "Completed within timeout"
            });
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
        {
            stopwatch.Stop();
            _logger.LogWarning(
                "WithTimeout exceeded {TimeoutSeconds}s timeout after {ElapsedMs}ms",
                timeoutSeconds, stopwatch.ElapsedMilliseconds);

            return StatusCode(408, new
            {
                error = "Request timeout",
                timeoutSeconds,
                iterations,
                elapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                message = "Operation exceeded the specified timeout"
            });
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();
            _logger.LogInformation("WithTimeout cancelled by client after {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);

            return StatusCode(499, new
            {
                error = "Client cancelled request",
                timeoutSeconds,
                iterations,
                elapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                message = "Request was cancelled by the client"
            });
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error in WithTimeout");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Demonstrates cancellation with cleanup (using/finally blocks)
    /// </summary>
    /// <param name="iterations">Number of iterations</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result with cleanup information</returns>
    /// <remarks>
    /// **Pattern**: Cancellation with resource cleanup
    ///
    /// **What this shows**: How to ensure cleanup happens even when operations are cancelled.
    ///
    /// **Key techniques**:
    /// - Use `using` statements for automatic cleanup
    /// - Put cleanup code in `finally` blocks
    /// - Cleanup runs whether operation completes, fails, or is cancelled
    ///
    /// **Real-world examples**:
    /// - Closing database connections
    /// - Releasing file handles
    /// - Disposing HTTP clients
    /// - Releasing semaphore slots
    ///
    /// **What can go wrong**: Without proper cleanup, cancellation can leak resources (connections, memory, locks).
    /// </remarks>
    [HttpGet("with-cleanup")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(499)]
    public async Task<IActionResult> WithCleanup(
        [FromQuery] int iterations = 100,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        bool cleanupExecuted = false;
        string cleanupMessage = "";

        _logger.LogInformation("Starting WithCleanup with {Iterations} iterations", iterations);

        try
        {
            var result = await _mockService.LongRunningOperationWithCancellationTokenAsync(
                iterations,
                cancellationToken);

            stopwatch.Stop();

            return Ok(new
            {
                result,
                iterations,
                elapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                cleanupExecuted,
                cleanupMessage = cleanupExecuted ? cleanupMessage : "Cleanup in finally block"
            });
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();
            _logger.LogWarning("WithCleanup cancelled after {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);

            return StatusCode(499, new
            {
                message = "Operation cancelled",
                iterations,
                elapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                cleanupExecuted,
                cleanupMessage = "Cleanup executed despite cancellation"
            });
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error in WithCleanup");
            return StatusCode(500, new { error = ex.Message });
        }
        finally
        {
            // This ALWAYS runs - whether we complete, fail, or cancel
            cleanupExecuted = true;
            cleanupMessage = $"Cleanup executed at {stopwatch.ElapsedMilliseconds}ms";
            _logger.LogInformation("Cleanup executed for WithCleanup after {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);

            // In real code, you would:
            // - Dispose resources
            // - Close connections
            // - Release locks/semaphores
            // - Log cleanup completion
        }
    }
}
