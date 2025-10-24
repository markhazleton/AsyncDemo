namespace AsyncDemo.Web.Controllers;

/// <summary>
/// BaseController - Refactored to follow DI best practices
/// </summary>
[ApiExplorerSettings(IgnoreApi = true)]
public abstract class BaseController : Controller
{
    /// <summary>
    /// Logger for base controller
    /// </summary>
    protected readonly ILogger _logger;

    /// <summary>
    /// HTTP Client Factory for creating HTTP clients
    /// </summary>
    protected readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    /// Weather Policy
    /// </summary>
    protected readonly AsyncRetryPolicy<HttpResponseMessage> _httpWeatherPolicy;

    /// <summary>
    /// Shared Jitter for retry policies
    /// </summary>
    protected static readonly Random jitter = new();

    /// <summary>
    /// Base Controller Constructor
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="httpClientFactory">HTTP client factory</param>
    protected BaseController(ILogger logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

        _httpWeatherPolicy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
          .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt) / 2)
  + TimeSpan.FromSeconds(jitter.Next(0, 3)));
    }

    /// <summary>
    /// Creates a properly configured HTTP client
    /// </summary>
    /// <returns>HttpClient instance</returns>
    protected HttpClient CreateHttpClient()
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return client;
    }

    /// <summary>
    /// Creates a cancellation token source with the specified timeout
    /// </summary>
    /// <param name="timeout">Timeout duration</param>
    /// <returns>CancellationTokenSource</returns>
    protected static CancellationTokenSource CreateCancellationTokenSource(TimeSpan? timeout = null)
    {
        return timeout.HasValue ? new CancellationTokenSource(timeout.Value) : new CancellationTokenSource();
    }
}
