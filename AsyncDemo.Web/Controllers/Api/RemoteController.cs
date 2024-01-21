
namespace AsyncDemo.Web.Controllers.Api;

/// <summary>
/// Remote Server MOCK
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RemoteController"/> class.
/// </remarks>
/// <param name="logger">The logger.</param>
/// <param name="memoryCache">The memory cache.</param>
[ApiController]
[Route("api/remote")]
public class RemoteController(ILogger<RemoteController> logger, IMemoryCache memoryCache) : BaseApiController(memoryCache)
{
    private readonly ILogger<RemoteController> _logger = logger;
    private readonly AsyncMockService asyncMock = new();

    /// <summary>
    /// Asynchronously performs the long running operation and returns the mock results.
    /// </summary>
    /// <param name="loopCount">The loop count.</param>
    /// <returns>The mock results.</returns>
    private async Task<MockResults> MockResultsAsync(int loopCount)
    {
        MockResults returnMock = new() { LoopCount = loopCount, Message = "init" };
        using (var cancellationTokenSource = new CancellationTokenSource())
        {
            try
            {
                // Running the long running task
                var result = await asyncMock.LongRunningOperationWithCancellationTokenAsync(loopCount, cancellationTokenSource.Token)
                    .ConfigureAwait(false);
                returnMock.Message = $"Task Complete";
                returnMock.ResultValue = result.ToString();
            }
            catch (TaskCanceledException)
            {
                returnMock.Message = "TaskCanceledException";
                returnMock.ResultValue = "-1";
            }
        }
        return returnMock;
    }

    /// <summary>
    /// Posts the results.
    /// </summary>
    /// <param name="model">The instance of the request model.</param>
    /// <returns>The action result.</returns>
    /// <response code="200">Request Processed successfully.</response>
    /// <response code="200">Request Timeout.</response>
    [ProducesResponseType(typeof(MockResults), 200)]
    [ProducesResponseType(typeof(MockResults), 408)]
    [HttpPost]
    [Route("Results")]
    public async Task<IActionResult> GetResults(MockResults model)
    {
        Stopwatch watch = new();
        watch.Start();

        MockResults myResult = new() { LoopCount = model.LoopCount, MaxTimeMS = model.MaxTimeMS, Message = "init" };
        var listOfTasks = new List<Task>();
        var task1 = MockResultsAsync(model.LoopCount);
        listOfTasks.Add(task1);
        var taskResults = await Task.WhenAll(listOfTasks.Select(x => Task.WhenAny(x, Task.Delay(TimeSpan.FromMilliseconds(model.MaxTimeMS)))));
        var succeedResults = taskResults.OfType<Task<MockResults>>().Select(s => s.Result).ToList();

        watch.Stop();
        myResult.RunTimeMS = (int)watch.ElapsedMilliseconds;
        if (succeedResults.Count != listOfTasks.Count)
        {
            myResult.Message = "Time Out Occurred";
            myResult.ResultValue = "-1";
            return StatusCode((int)HttpStatusCode.RequestTimeout, myResult);
        }

        myResult.Message = succeedResults.FirstOrDefault()?.Message ?? String.Empty;
        myResult.ResultValue = succeedResults.FirstOrDefault()?.ResultValue ?? String.Empty;
        _logger.LogInformation("GetResults:OK", myResult);
        return Ok(myResult);
    }
}
