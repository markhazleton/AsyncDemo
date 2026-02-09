using AsyncSpark.HttpGetCall;

namespace AsyncSpark.Web.Controllers;

/// <summary>
/// Controller for handling bulk HTTP GET calls.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="BulkCallsController"/> class.
/// </remarks>
/// <param name="logger">The logger.</param>
/// <param name="getCallService">The HTTP GET call service.</param>
/// <param name="httpClientFactory">The HTTP client factory.</param>
public class BulkCallsController(ILogger<BulkCallsController> logger, IHttpGetCallService getCallService, IHttpClientFactory httpClientFactory) : BaseController(logger, httpClientFactory)
{
    private static readonly object WriteLock = new();

    /// <summary>
    /// Calls the specified endpoint multiple times asynchronously.
    /// </summary>
    /// <param name="maxThreads">The maximum number of concurrent threads.</param>
    /// <param name="iterationCount">The number of iterations.</param>
    /// <param name="endpoint">The endpoint URL.</param>
    /// <returns>A list of HTTP GET call results.</returns>
    private async Task<List<HttpGetCallResults>> CallEndpointMultipleTimes(int maxThreads = 1, int iterationCount = 10, string endpoint = "https://AsyncSpark.markhazleton.com/status")
    {
        int curIndex = 0;
        // Create a SemaphoreSlim with a maximum of maxThreads concurrent requests
        SemaphoreSlim semaphore = new(maxThreads);
        List<HttpGetCallResults> results = [];
        var cts = CreateCancellationTokenSource(TimeSpan.FromSeconds(2));

        // Create a list of tasks to make the GetAsync calls
        List<Task> tasks = [];
        for (int i = 0; i < iterationCount; i++)
        {
            // Acquire the semaphore before making the request
            await semaphore.WaitAsync();
            curIndex++;
            var statusCall = new HttpGetCallResults(curIndex, endpoint);
            // Create a task to make the request
            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    // Get the async results
                    var result = await getCallService.GetAsync<ApplicationStatus>(statusCall, cts.Token);
                    lock (WriteLock)
                    {
                        results.Add(result);
                    }
                }
                finally
                {
                    // Release the semaphore
                    semaphore.Release();
                }
            }));
        }

        // Wait for all tasks to complete
        await Task.WhenAll(tasks);

        // Log a message when all calls are complete
        _logger.LogInformation("All calls complete");
        return results;
    }

    // GET: BulkCallsController
    /// <summary>
    /// Action method for the index page.
    /// </summary>
    /// <returns>The index view with the results of the bulk HTTP GET calls.</returns>
    public async Task<ActionResult> Index()
    {
        var results = await CallEndpointMultipleTimes();
        return View(results);
    }

    // GET: BulkCallsController/Create
    /// <summary>
    /// Action method for creating a new bulk call test.
    /// </summary>
    /// <returns>The create view.</returns>
    public ActionResult Create()
    {
        ViewBag.MaxThreads = 5;
        ViewBag.IterationCount = 10;
        ViewBag.Endpoint = "https://AsyncSpark.markhazleton.com/status";
        return View();
    }

    // POST: BulkCallsController/Create
    /// <summary>
    /// Action method for handling the form submission to create a new bulk call test.
    /// </summary>
    /// <param name="maxThreads">The maximum number of concurrent threads.</param>
    /// <param name="iterationCount">The number of iterations.</param>
    /// <param name="endpoint">The endpoint URL.</param>
    /// <returns>Redirects to the index view with the results of the bulk HTTP GET calls.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create(int maxThreads, int iterationCount, string endpoint)
    {
        try
        {
            if (string.IsNullOrEmpty(endpoint))
            {
                endpoint = "https://AsyncSpark.markhazleton.com/status";
            }

            var results = await CallEndpointMultipleTimes(maxThreads, iterationCount, endpoint);
            TempData["BulkCallSuccess"] = $"Successfully completed {results.Count} API calls out of {iterationCount} requested";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing bulk calls");
            TempData["BulkCallError"] = $"Error: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }
}
