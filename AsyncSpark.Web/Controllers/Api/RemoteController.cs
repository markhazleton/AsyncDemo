namespace AsyncSpark.Web.Controllers.Api;

/// <summary>
/// Remote Server MOCK - Demonstrates timeout and cancellation patterns
/// </summary>
[ApiController]
[Route("api/remote")]
[Consumes("application/json")]
[Tags("4. Resilience & Timeouts")]
public class RemoteController(ILogger<RemoteController> _logger, IMemoryCache memoryCache) : BaseApiController(memoryCache)
{
    private readonly AsyncMockService _asyncMock = new();

    /// <summary>
    /// Asynchronously performs the long-running operation and returns the mock results.
    /// </summary>
    /// <param name="loopCount">The loop count.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>The mock results.</returns>
    private async Task<MockResults> MockResultsAsync(int loopCount, CancellationToken cancellationToken)
    {
        var returnMock = new MockResults(loopCount, 0);

        try
        {
            // Running the long-running task with the cancellation token
            var result = await _asyncMock.LongRunningOperationWithCancellationTokenAsync(loopCount, cancellationToken)
                .ConfigureAwait(false);
            returnMock.Message = "Task Complete";
            returnMock.ResultValue = result.ToString();
        }
        catch (TaskCanceledException)
        {
            _logger.LogWarning("Task was canceled for LoopCount {LoopCount}", loopCount);
            throw; // Rethrow the exception to be caught at a higher level
        }
        catch (Exception ex)
        {
            returnMock.Message = $"Error: {ex.Message}";
            returnMock.ResultValue = "-1";
            _logger.LogError(ex, "Error occurred in MockResultsAsync");
        }
        return returnMock;
    }


    /// <summary>
    /// Mock long-running operation with timeout handling
    /// </summary>
    /// <param name="model">Request model with LoopCount and MaxTimeMS</param>
    /// <returns>Results or timeout response</returns>
    /// <remarks>
    /// **Pattern**: Timeout with CancellationTokenSource
    ///
    /// **What this shows**: How to set operation-specific timeouts using `CancellationTokenSource(TimeSpan)`.
    ///
    /// **Key technique**: Pass `MaxTimeMS` to create a timeout-based cancellation token,
    /// then wire it through the async operation.
    ///
    /// **Returns**:
    /// - 200: Operation completed within timeout
    /// - 408: Operation exceeded timeout (Request Timeout)
    /// - 500: Other errors
    ///
    /// **Try it**: Set MaxTimeMS lower than LoopCount to trigger timeout.
    /// </remarks>
    /// <response code="200">Request processed successfully.</response>
    /// <response code="408">Request Timeout.</response>
    [ProducesResponseType(typeof(MockResults), 200)]
    [ProducesResponseType(typeof(MockResults), 408)]
    [Consumes("application/json")]
    [HttpPost]
    [Route("Results")]
    public async Task<IActionResult> GetResults([FromBody] MockResults model)
    {
        var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(model.MaxTimeMS));
        var watch = Stopwatch.StartNew();
        MockResults? result;
        try
        {
            // Pass the cancellation token to MockResultsAsync to allow it to respond to the cancellation
            result = await MockResultsAsync(model.LoopCount, cts.Token);
            result.MaxTimeMS = model.MaxTimeMS;
        }
        catch (OperationCanceledException)
        {
            watch.Stop();
            result = new MockResults(model.LoopCount, model.MaxTimeMS)
            {
                RunTimeMS = watch.ElapsedMilliseconds,
                Message = "Time Out Occurred",
                ResultValue = "-1"
            };

            _logger.LogWarning("GetResults: Request timeout for LoopCount {LoopCount} with MaxTimeMS {MaxTimeMS}", model.LoopCount, model.MaxTimeMS);
            return StatusCode((int)HttpStatusCode.RequestTimeout, result);
        }
        catch (Exception ex)
        {
            watch.Stop();
            result = new MockResults(model.LoopCount, model.MaxTimeMS)
            {
                RunTimeMS = watch.ElapsedMilliseconds,
                Message = $"Error: {ex.Message}",
                ResultValue = "-1"
            };

            _logger.LogError(ex, "GetResults: An error occurred for LoopCount {LoopCount} with MaxTimeMS {MaxTimeMS}", model.LoopCount, model.MaxTimeMS);
            return StatusCode((int)HttpStatusCode.InternalServerError, result);
        }

        watch.Stop();
        result.RunTimeMS = watch.ElapsedMilliseconds;

        _logger.LogInformation("GetResults: OK for LoopCount {LoopCount} with MaxTimeMS {MaxTimeMS}", model.LoopCount, model.MaxTimeMS);
        return Ok(result);
    }

}