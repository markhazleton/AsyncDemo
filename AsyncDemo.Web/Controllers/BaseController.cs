namespace AsyncDemo.Web.Controllers;

/// <summary>
/// BaseController
/// </summary>
[ApiExplorerSettings(IgnoreApi = true)]
public abstract class BaseController : Controller
{
    /// <summary>
    /// Retry Count
    /// </summary>
    protected const string retryCountKey = "retrycount";
    /// <summary>
    /// Index Policy
    /// </summary>
    protected readonly AsyncRetryPolicy<HttpResponseMessage> _httpIndexPolicy;
    /// <summary>
    /// Weather Policy
    /// </summary>
    protected readonly AsyncRetryPolicy<HttpResponseMessage> _httpWeatherPolicy;
    /// <summary>
    /// Shared Http Client
    /// </summary>
    protected readonly HttpClient _httpClient;
    /// <summary>
    /// Shared Stopwatch
    /// </summary>
    protected static readonly Stopwatch stopWatch;
    /// <summary>
    /// Shared Jitter
    /// </summary>
    protected static readonly Random jitter;
    /// <summary>
    /// 
    /// </summary>
    public readonly CancellationTokenSource cts;


    /// <summary>
    /// Static Base Constructor
    /// </summary>
    static BaseController()
    {
        stopWatch = new Stopwatch();
        jitter = new Random();
    }

    /// <summary>
    /// Base Controller Constructor
    /// </summary>
    public BaseController()
    {
        cts = new CancellationTokenSource();
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        _httpWeatherPolicy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt) / 2)
                + TimeSpan.FromSeconds(jitter.Next(0, 3)));

        _httpIndexPolicy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt) / 2)
                + TimeSpan.FromSeconds(jitter.Next(0, 3)));
    }

}
