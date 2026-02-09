namespace AsyncSpark.Web.Controllers.Api;

/// <summary>
/// Weather API Controller demonstrating async/await patterns
/// </summary>
/// <remarks>
/// This controller provides focused examples of cancellation, timeouts, and resilience patterns
/// using real-world weather API scenarios.
/// </remarks>
[ApiController]
[Route("api/weather")]
[Tags("1. Async Basics")]
public class WeatherPatternsController : ControllerBase
{
    private readonly ILogger<WeatherPatternsController> _logger;
    private readonly IOpenWeatherMapClient _weatherService;
    private readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    /// Initializes a new instance of the WeatherPatternsController
    /// </summary>
    /// <param name="logger">Logger for diagnostic output</param>
    /// <param name="weatherService">Weather API client service</param>
    /// <param name="httpClientFactory">HTTP client factory for making requests</param>
    public WeatherPatternsController(
        ILogger<WeatherPatternsController> logger,
        IOpenWeatherMapClient weatherService,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _weatherService = weatherService;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Basic async weather call - NO timeout protection
    /// </summary>
    /// <param name="location">City name (e.g., "Dallas", "London")</param>
    /// <returns>Current weather data</returns>
    /// <remarks>
    /// **Pattern**: Basic async/await without protection
    ///
    /// **What this shows**: A naive async call that could hang indefinitely if the API is slow.
    ///
    /// **What can go wrong**: If the weather API takes 5 minutes to respond (or never responds),
    /// this endpoint will wait forever, tying up server resources.
    ///
    /// **Compare with**: `/api/weather/with-timeout` to see the protected version.
    ///
    /// **Learning objective**: Always protect external calls with timeouts.
    /// </remarks>
    [HttpGet("slow")]
    [ProducesResponseType(typeof(CurrentWeather), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetWeatherSlow([FromQuery] string location = "Dallas")
    {
        try
        {
            _logger.LogInformation("GetWeatherSlow called for {Location} - NO timeout protection", location);

            var weather = await _weatherService.GetCurrentWeatherAsync(location);

            if (!weather.Success)
            {
                return BadRequest(new { error = weather.ErrorMessage, location });
            }

            return Ok(weather);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetWeatherSlow for {Location}", location);
            return StatusCode(500, new { error = ex.Message, location });
        }
    }

    /// <summary>
    /// Protected async weather call - WITH timeout and cancellation
    /// </summary>
    /// <param name="location">City name (e.g., "Dallas", "London")</param>
    /// <param name="timeoutSeconds">Timeout in seconds (default: 5)</param>
    /// <param name="cancellationToken">Cancellation token from HTTP request</param>
    /// <returns>Current weather data</returns>
    /// <remarks>
    /// **Pattern**: Async with timeout and cancellation token
    ///
    /// **What this shows**: How to protect an external API call with:
    /// 1. A timeout (fails fast if API is slow)
    /// 2. HTTP request cancellation (stops work if user cancels)
    ///
    /// **Key techniques**:
    /// - `CancellationTokenSource.CreateLinkedTokenSource()` combines HTTP and timeout cancellation
    /// - If either the user cancels OR timeout expires, the operation stops
    /// - Returns 408 Request Timeout for timeouts, 499 for user cancellation
    ///
    /// **Compare with**: `/api/weather/slow` to see the unprotected version.
    ///
    /// **Try it**: Call this endpoint then quickly cancel the request - notice the operation stops immediately.
    /// </remarks>
    [HttpGet("with-timeout")]
    [ProducesResponseType(typeof(CurrentWeather), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(408)]
    [ProducesResponseType(499)]
    public async Task<IActionResult> GetWeatherWithTimeout(
        [FromQuery] string location = "Dallas",
        [FromQuery] int timeoutSeconds = 5,
        CancellationToken cancellationToken = default)
    {
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        try
        {
            _logger.LogInformation(
                "GetWeatherWithTimeout called for {Location} with {TimeoutSeconds}s timeout",
                location, timeoutSeconds);

            var weather = await _weatherService.GetCurrentWeatherAsync(location);

            // Check for cancellation after the call
            linkedCts.Token.ThrowIfCancellationRequested();

            if (!weather.Success)
            {
                return BadRequest(new { error = weather.ErrorMessage, location });
            }

            return Ok(weather);
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
        {
            _logger.LogWarning("Timeout occurred for {Location} after {TimeoutSeconds}s", location, timeoutSeconds);
            return StatusCode(408, new
            {
                error = $"Request timed out after {timeoutSeconds} seconds",
                location,
                timeoutSeconds
            });
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Request cancelled by user for {Location}", location);
            return StatusCode(499, new { error = "Request cancelled by client", location });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetWeatherWithTimeout for {Location}", location);
            return StatusCode(500, new { error = ex.Message, location });
        }
    }

    /// <summary>
    /// Weather call with Polly retry policy - handles transient failures
    /// </summary>
    /// <param name="location">City name (e.g., "Dallas", "London")</param>
    /// <param name="maxRetries">Maximum retry attempts (default: 3)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Current weather data with retry information</returns>
    /// <remarks>
    /// **Pattern**: Resilience with Polly retry policies
    ///
    /// **What this shows**: How to handle transient failures (network blips, temporary API errors) by retrying with exponential backoff.
    ///
    /// **Key techniques**:
    /// - Polly retry policy with exponential backoff (1s, 2s, 4s...)
    /// - Jitter prevents "retry storms" when many clients retry simultaneously
    /// - Context tracks retry count for observability
    ///
    /// **When to use**: Any call to external services that might have transient failures.
    ///
    /// **What can go wrong**: Without retries, temporary network issues become permanent failures for your users.
    ///
    /// **Try it**: The response includes `retryCount` so you can see if retries occurred.
    /// </remarks>
    [HttpGet("with-retry")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(408)]
    public async Task<IActionResult> GetWeatherWithRetry(
        [FromQuery] string location = "Dallas",
        [FromQuery] int maxRetries = 3,
        CancellationToken cancellationToken = default)
    {
        var random = new Random();
        int retryCount = 0;

        var retryPolicy = Policy
            .Handle<Exception>(ex => !(ex is OperationCanceledException))
            .WaitAndRetryAsync(
                maxRetries,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) // Exponential backoff
                              + TimeSpan.FromMilliseconds(random.Next(0, 1000)), // Jitter
                onRetry: (exception, timespan, retry, context) =>
                {
                    retryCount = retry;
                    _logger.LogWarning(
                        "Retry {RetryCount}/{MaxRetries} for {Location} after {Delay}ms. Error: {Error}",
                        retry, maxRetries, location, timespan.TotalMilliseconds, exception.Message);
                });

        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);

            var weather = await retryPolicy.ExecuteAsync(async () =>
            {
                linkedCts.Token.ThrowIfCancellationRequested();
                return await _weatherService.GetCurrentWeatherAsync(location);
            });

            if (!weather.Success)
            {
                return BadRequest(new
                {
                    error = weather.ErrorMessage,
                    location,
                    retryCount,
                    maxRetries
                });
            }

            return Ok(new
            {
                weather,
                retryInfo = new
                {
                    retriesPerformed = retryCount,
                    maxRetriesAllowed = maxRetries,
                    succeeded = true
                }
            });
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Request cancelled for {Location} after {RetryCount} retries", location, retryCount);
            return StatusCode(408, new
            {
                error = "Request cancelled or timed out",
                location,
                retriesPerformed = retryCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "All retries exhausted for {Location}", location);
            return StatusCode(500, new
            {
                error = "All retry attempts failed",
                lastError = ex.Message,
                location,
                retriesPerformed = retryCount,
                maxRetries
            });
        }
    }

    /// <summary>
    /// Fetch weather for multiple cities concurrently using Task.WhenAll
    /// </summary>
    /// <param name="locations">Comma-separated city names (e.g., "Dallas,London,Tokyo")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Weather data for all cities</returns>
    /// <remarks>
    /// **Pattern**: Concurrent async operations with Task.WhenAll
    ///
    /// **What this shows**: How to fetch data for multiple cities in parallel instead of sequentially.
    ///
    /// **Key difference**:
    /// - ❌ BAD: `foreach` loop with `await` inside = sequential (slow)
    /// - ✅ GOOD: `Task.WhenAll` = parallel (fast)
    ///
    /// **Performance impact**: For 3 cities with 1s API calls each:
    /// - Sequential: ~3 seconds total
    /// - Parallel: ~1 second total
    ///
    /// **What can go wrong**: If one city fails, `Task.WhenAll` throws. Consider using continuation-based error handling for partial success scenarios.
    ///
    /// **Try it**: Request weather for 5+ cities and compare response time to sequential calls.
    /// </remarks>
    [HttpGet("multiple")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetMultipleCitiesWeather(
        [FromQuery] string locations = "Dallas,London,Tokyo",
        CancellationToken cancellationToken = default)
    {
        var cities = locations.Split(',', StringSplitOptions.RemoveEmptyEntries)
                              .Select(c => c.Trim())
                              .ToList();

        if (!cities.Any())
        {
            return BadRequest(new { error = "No locations provided" });
        }

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("Fetching weather for {Count} cities in parallel: {Cities}",
                cities.Count, string.Join(", ", cities));

            // Create tasks for all cities
            var weatherTasks = cities.Select(async city =>
            {
                linkedCts.Token.ThrowIfCancellationRequested();
                var weather = await _weatherService.GetCurrentWeatherAsync(city);
                return new { city, weather, success = weather.Success };
            }).ToList();

            // Execute all tasks in parallel
            var results = await Task.WhenAll(weatherTasks);

            stopwatch.Stop();

            var successful = results.Where(r => r.success).ToList();
            var failed = results.Where(r => !r.success).ToList();

            return Ok(new
            {
                summary = new
                {
                    totalCities = cities.Count,
                    successful = successful.Count,
                    failed = failed.Count,
                    elapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                    parallelExecution = true
                },
                results = results.Select(r => new
                {
                    r.city,
                    r.success,
                    weather = r.success ? r.weather : null,
                    error = !r.success ? r.weather.ErrorMessage : null
                })
            });
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();
            _logger.LogWarning("Multiple cities request cancelled after {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
            return StatusCode(408, new
            {
                error = "Request cancelled or timed out",
                elapsedMilliseconds = stopwatch.ElapsedMilliseconds
            });
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error fetching weather for multiple cities");
            return StatusCode(500, new
            {
                error = ex.Message,
                elapsedMilliseconds = stopwatch.ElapsedMilliseconds
            });
        }
    }
}
