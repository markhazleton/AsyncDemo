namespace AsyncDemo.Web.Controllers.Api;

/// <summary>
/// API Controller demonstrating concurrency patterns and parallel execution
/// </summary>
[ApiController]
[Route("api/concurrency")]
[Tags("3. Concurrency & Parallelism")]
public class ConcurrencyPatternsController : ControllerBase
{
    private readonly ILogger<ConcurrencyPatternsController> _logger;
    private readonly AsyncMockService _mockService;

    public ConcurrencyPatternsController(ILogger<ConcurrencyPatternsController> logger)
    {
        _logger = logger;
        _mockService = new AsyncMockService();
    }

    /// <summary>
    /// Sequential execution - operations run one after another (SLOW)
    /// </summary>
    /// <param name="operationCount">Number of operations to perform</param>
    /// <param name="iterationsPerOperation">Iterations for each operation</param>
    /// <returns>Results showing sequential execution time</returns>
    /// <remarks>
    /// **Pattern**: Sequential async (anti-pattern for independent operations)
    ///
    /// **What this shows**: Using `await` in a loop makes operations run sequentially.
    ///
    /// **Performance**: For 5 operations taking 1 second each = ~5 seconds total
    ///
    /// **When this is OK**: When operations MUST run in order (each depends on previous result)
    ///
    /// **When this is BAD**: When operations are independent (like fetching data for different users)
    ///
    /// **Compare with**: `/api/concurrency/parallel` to see the performance difference.
    ///
    /// **Try it**: Call with operationCount=5 and watch the elapsed time.
    /// </remarks>
    [HttpGet("sequential")]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<IActionResult> Sequential(
        [FromQuery] int operationCount = 5,
        [FromQuery] int iterationsPerOperation = 50)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var results = new List<object>();

        _logger.LogInformation("Starting {Count} operations SEQUENTIALLY", operationCount);

        for (int i = 0; i < operationCount; i++)
        {
            var opStopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Each await blocks until complete before moving to next
            var result = await _mockService.LongRunningOperation(iterationsPerOperation);

            opStopwatch.Stop();

            results.Add(new
            {
                operationNumber = i + 1,
                result,
                elapsedMilliseconds = opStopwatch.ElapsedMilliseconds
            });

            _logger.LogInformation("Operation {Number} completed in {ElapsedMs}ms", i + 1, opStopwatch.ElapsedMilliseconds);
        }

        stopwatch.Stop();

        return Ok(new
        {
            executionMode = "Sequential",
            totalOperations = operationCount,
            iterationsPerOperation,
            totalElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
            averageMillisecondsPerOperation = stopwatch.ElapsedMilliseconds / operationCount,
            results,
            explanation = "Each operation waited for the previous one to complete. This is SLOW for independent operations."
        });
    }

    /// <summary>
    /// Parallel execution using Task.WhenAll - operations run concurrently (FAST)
    /// </summary>
    /// <param name="operationCount">Number of operations to perform</param>
    /// <param name="iterationsPerOperation">Iterations for each operation</param>
    /// <returns>Results showing parallel execution time</returns>
    /// <remarks>
    /// **Pattern**: Task.WhenAll for parallel execution
    ///
    /// **What this shows**: How to run independent operations concurrently.
    ///
    /// **Performance**: For 5 operations taking 1 second each = ~1 second total (5x faster!)
    ///
    /// **Key technique**:
    /// 1. Create tasks WITHOUT awaiting them
    /// 2. Collect tasks in a list
    /// 3. Use `Task.WhenAll()` to await all at once
    ///
    /// **Real-world use cases**:
    /// - Fetching data for multiple users
    /// - Calling multiple microservices
    /// - Processing multiple files
    /// - Sending notifications to multiple recipients
    ///
    /// **Compare with**: `/api/concurrency/sequential` to see the speedup.
    /// </remarks>
    [HttpGet("parallel")]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<IActionResult> Parallel(
        [FromQuery] int operationCount = 5,
        [FromQuery] int iterationsPerOperation = 50)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var tasks = new List<Task<object>>();

        _logger.LogInformation("Starting {Count} operations in PARALLEL", operationCount);

        // Create all tasks WITHOUT awaiting (they start running immediately)
        for (int i = 0; i < operationCount; i++)
        {
            int operationNumber = i + 1; // Capture for closure
            var opStopwatch = System.Diagnostics.Stopwatch.StartNew();

            var task = _mockService.LongRunningOperation(iterationsPerOperation)
                .ContinueWith(t =>
                {
                    opStopwatch.Stop();
                    return (object)new
                    {
                        operationNumber,
                        result = t.Result,
                        elapsedMilliseconds = opStopwatch.ElapsedMilliseconds
                    };
                });

            tasks.Add(task);
        }

        // Now await ALL tasks at once
        var results = await Task.WhenAll(tasks);

        stopwatch.Stop();

        _logger.LogInformation("All {Count} parallel operations completed in {ElapsedMs}ms", operationCount, stopwatch.ElapsedMilliseconds);

        return Ok(new
        {
            executionMode = "Parallel (Task.WhenAll)",
            totalOperations = operationCount,
            iterationsPerOperation,
            totalElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
            speedupVsSequential = $"~{operationCount}x faster than sequential",
            results,
            explanation = "All operations ran concurrently. Total time ≈ slowest operation, not sum of all operations."
        });
    }

    /// <summary>
    /// Throttled parallel execution using SemaphoreSlim - controlled concurrency
    /// </summary>
    /// <param name="operationCount">Number of operations to perform</param>
    /// <param name="maxConcurrency">Maximum concurrent operations</param>
    /// <param name="iterationsPerOperation">Iterations for each operation</param>
    /// <returns>Results showing throttled execution</returns>
    /// <remarks>
    /// **Pattern**: SemaphoreSlim for throttling
    ///
    /// **What this shows**: How to limit concurrent operations to avoid overwhelming resources.
    ///
    /// **Why you need this**:
    /// - Database connection pools have limits (e.g., 100 connections)
    /// - External APIs have rate limits (e.g., 10 requests/second)
    /// - Don't want to exhaust memory/CPU with too many parallel operations
    ///
    /// **How it works**:
    /// - `SemaphoreSlim(maxConcurrency)` acts like a bouncer at a club
    /// - Only `maxConcurrency` operations run at once
    /// - When one finishes, the next one starts
    ///
    /// **Try it**:
    /// - Set operationCount=10, maxConcurrency=2
    /// - You'll see operations complete in waves of 2
    ///
    /// **Real-world example**: Preventing connection pool exhaustion when processing large batches.
    /// </remarks>
    [HttpGet("throttled")]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<IActionResult> Throttled(
        [FromQuery] int operationCount = 10,
        [FromQuery] int maxConcurrency = 3,
        [FromQuery] int iterationsPerOperation = 50)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var semaphore = new SemaphoreSlim(maxConcurrency);
        var results = new List<object>();
        var resultsLock = new object();

        _logger.LogInformation(
            "Starting {TotalOps} operations with max {MaxConcurrency} concurrent",
            operationCount, maxConcurrency);

        var tasks = new List<Task>();

        for (int i = 0; i < operationCount; i++)
        {
            int operationNumber = i + 1;

            var task = Task.Run(async () =>
            {
                // Wait for semaphore (blocks if at max concurrency)
                await semaphore.WaitAsync();
                var opStopwatch = System.Diagnostics.Stopwatch.StartNew();

                try
                {
                    _logger.LogInformation("Operation {Number} started", operationNumber);

                    var result = await _mockService.LongRunningOperation(iterationsPerOperation);

                    opStopwatch.Stop();

                    lock (resultsLock)
                    {
                        results.Add(new
                        {
                            operationNumber,
                            result,
                            elapsedMilliseconds = opStopwatch.ElapsedMilliseconds
                        });
                    }

                    _logger.LogInformation("Operation {Number} completed in {ElapsedMs}ms", operationNumber, opStopwatch.ElapsedMilliseconds);
                }
                finally
                {
                    // ALWAYS release semaphore (even if exception occurs)
                    semaphore.Release();
                }
            });

            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
        stopwatch.Stop();

        return Ok(new
        {
            executionMode = "Throttled (SemaphoreSlim)",
            totalOperations = operationCount,
            maxConcurrency,
            iterationsPerOperation,
            totalElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
            results = results.OrderBy(r => ((dynamic)r).operationNumber),
            explanation = $"Only {maxConcurrency} operations ran concurrently at any time. Prevents resource exhaustion."
        });
    }

    /// <summary>
    /// Compare sequential vs parallel vs throttled execution side-by-side
    /// </summary>
    /// <param name="operationCount">Number of operations</param>
    /// <param name="maxConcurrency">Max concurrent for throttled mode</param>
    /// <param name="iterationsPerOperation">Iterations per operation</param>
    /// <returns>Comparison of all three approaches</returns>
    /// <remarks>
    /// **Pattern**: Performance comparison
    ///
    /// **What this shows**: Side-by-side comparison of three approaches to running multiple async operations.
    ///
    /// **Expected results** (for 5 operations @ 1s each):
    /// - Sequential: ~5 seconds (1+1+1+1+1)
    /// - Parallel: ~1 second (max of all)
    /// - Throttled (max=2): ~3 seconds (1+1+1 in waves)
    ///
    /// **Choosing the right approach**:
    /// - Sequential: Operations MUST run in order
    /// - Parallel: Operations are independent AND no resource limits
    /// - Throttled: Operations are independent BUT must respect resource limits
    ///
    /// **Try it**: Experiment with different values to see the tradeoffs.
    /// </remarks>
    [HttpGet("comparison")]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<IActionResult> Comparison(
        [FromQuery] int operationCount = 5,
        [FromQuery] int maxConcurrency = 2,
        [FromQuery] int iterationsPerOperation = 50)
    {
        var totalStopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Sequential
        var seqStopwatch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < operationCount; i++)
        {
            await _mockService.LongRunningOperation(iterationsPerOperation);
        }
        seqStopwatch.Stop();

        // Parallel
        var parStopwatch = System.Diagnostics.Stopwatch.StartNew();
        var parTasks = Enumerable.Range(0, operationCount)
            .Select(_ => _mockService.LongRunningOperation(iterationsPerOperation))
            .ToList();
        await Task.WhenAll(parTasks);
        parStopwatch.Stop();

        // Throttled
        var thrStopwatch = System.Diagnostics.Stopwatch.StartNew();
        var semaphore = new SemaphoreSlim(maxConcurrency);
        var thrTasks = Enumerable.Range(0, operationCount)
            .Select(async _ =>
            {
                await semaphore.WaitAsync();
                try
                {
                    return await _mockService.LongRunningOperation(iterationsPerOperation);
                }
                finally
                {
                    semaphore.Release();
                }
            })
            .ToList();
        await Task.WhenAll(thrTasks);
        thrStopwatch.Stop();

        totalStopwatch.Stop();

        return Ok(new
        {
            parameters = new
            {
                operationCount,
                maxConcurrency,
                iterationsPerOperation
            },
            results = new
            {
                sequential = new
                {
                    milliseconds = seqStopwatch.ElapsedMilliseconds,
                    description = "Each operation waited for previous to complete"
                },
                parallel = new
                {
                    milliseconds = parStopwatch.ElapsedMilliseconds,
                    speedup = $"{seqStopwatch.ElapsedMilliseconds / (double)parStopwatch.ElapsedMilliseconds:F1}x",
                    description = "All operations ran concurrently"
                },
                throttled = new
                {
                    milliseconds = thrStopwatch.ElapsedMilliseconds,
                    speedup = $"{seqStopwatch.ElapsedMilliseconds / (double)thrStopwatch.ElapsedMilliseconds:F1}x",
                    description = $"Max {maxConcurrency} operations ran concurrently"
                }
            },
            totalElapsedMilliseconds = totalStopwatch.ElapsedMilliseconds,
            recommendation = operationCount <= maxConcurrency
                ? "For this count, parallel and throttled have similar performance"
                : $"Throttled is slower than parallel but safer for resource limits"
        });
    }
}
