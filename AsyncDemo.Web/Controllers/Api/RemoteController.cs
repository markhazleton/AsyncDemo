
namespace AsyncDemo.Web.Controllers.Api;

/// <summary>
/// Remote Server MOCK
/// </summary>
[ApiController]
[Route("api/remote")]
public class RemoteController : BaseApiController
{
    private readonly ILogger<RemoteController> _logger;
    private readonly AsyncMockService asyncMock = new();

    /// <summary>
    ///
    /// </summary>
    /// <param name="logger"></param>
    public RemoteController(ILogger<RemoteController> logger)
    {
        _logger = logger;
    }

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
    /// Post Results
    /// </summary>
    /// <param name="model">Instance of the requestModel</param>
    /// <returns></returns>
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
