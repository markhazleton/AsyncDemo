using AsyncDemo.HttpGetCall;

namespace AsyncDemo.Web.Controllers;

/// <summary>
/// 
/// </summary>
public class BulkCallsController : BaseController
{
    private readonly ILogger<BulkCallsController> _logger;
    private readonly IHttpGetCallService _service;
    private static readonly Object WriteLock = new();
    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="getCallService"></param>
    public BulkCallsController(ILogger<BulkCallsController> logger, IHttpGetCallService getCallService)
    {
        _logger = logger;
        _service = getCallService;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="maxThreads"></param>
    /// <param name="itterationCount"></param>
    /// <param name="endpoint"></param>
    /// <returns></returns>
    private async Task<List<HttpGetCallResults>> CallEndpointMultipleTimes(int maxThreads = 1, int itterationCount = 10, string endpoint = "https://asyncdemoweb.azurewebsites.net/status")
    {
        int curIndex = 0;
        // Create a SemaphoreSlim with a maximum of maxThreads concurrent requests
        SemaphoreSlim semaphore = new(maxThreads);
        List<HttpGetCallResults> results = new();

        // Create a list of tasks to make the GetAsync calls
        List<Task> tasks = new();
        for (int i = 0; i < itterationCount; i++)
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
                    // Get The Async Results
                    var result = await _service.GetAsync<ApplicationStatus>(statusCall);
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
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<ActionResult> Index()
    {
        var results = await CallEndpointMultipleTimes();
        return View(results);
    }
}
