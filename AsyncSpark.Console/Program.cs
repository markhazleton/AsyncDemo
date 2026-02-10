using AsyncSpark;
using AsyncSpark.Console.Demos;
using AsyncSpark.FireAndForget;
using Microsoft.Extensions.Logging;

Console.WriteLine($"Program Start: Thread:{Environment.CurrentManagedThreadId}");
Console.WriteLine();

// Demo 1: Cancellation Token with timeout
await CancellationTokenDemo.RunAsync(timeoutMilliseconds: 2000);

// Demo 2: Fire and Forget patterns
Console.WriteLine("=== Fire and Forget Demo ===");
var logger = new ConsoleLogger();
var adapterLogger = new LoggerAdapter<FireAndForgetUtility>(logger);
var cts = new CancellationTokenSource();
var fireAndForget = new FireAndForgetUtility(adapterLogger);

try
{
    fireAndForget.SafeFireAndForget(AsyncMockService.LongRunningTask("A.", delay: 5000, iterations: 1, throwEx: false, logger, cts.Token), cts.Token);
    fireAndForget.SafeFireAndForget(AsyncMockService.LongRunningTask("B.", delay: 4000, iterations: 1, throwEx: false, logger, cts.Token), cts.Token);
    fireAndForget.SafeFireAndForget(AsyncMockService.LongRunningTask("C.", delay: 3000, iterations: 1, throwEx: false, logger, cts.Token), cts.Token);
    fireAndForget.SafeFireAndForget(AsyncMockService.LongRunningTask("D.", delay: 2000, iterations: 1, throwEx: false, logger, cts.Token), cts.Token);
    fireAndForget.SafeFireAndForget(AsyncMockService.LongRunningTask("E.", delay: 1000, iterations: 1, throwEx: false, logger, cts.Token), cts.Token);
    fireAndForget.SafeFireAndForget(AsyncMockService.LongRunningTask("F.", delay: 1000, iterations: 1, throwEx: true, logger, cts.Token), cts.Token);
    fireAndForget.SafeFireAndForget(AsyncMockService.LongRunningTask("G.", delay: 500, iterations: 1, throwEx: false, logger, cts.Token), cts.Token);
    fireAndForget.SafeFireAndForget(AsyncMockService.LongRunningTask("H.", delay: 500, iterations: 2, throwEx: false, logger, cts.Token), cts.Token);
    fireAndForget.SafeFireAndForget(AsyncMockService.LongRunningTask("I.", delay: 500, iterations: 4, throwEx: false, logger, cts.Token), cts.Token);
    fireAndForget.SafeFireAndForget(AsyncMockService.LongRunningTask("J.", delay: 500, iterations: 8, throwEx: false, logger, cts.Token), cts.Token);
    fireAndForget.SafeFireAndForget(AsyncMockService.LongRunningTask("K.", delay: 1000, iterations: 20, throwEx: false, logger, cts.Token), cts.Token);

    // Allow some time for tasks to run before cancellation
    await Task.Delay(10000);
    cts.Cancel(); // Cancel remaining tasks
    await Task.Delay(5000); // Wait to observe cancellation effects

}
catch (Exception ex)
{
    logger.LogError(ex, "An exception occurred in the main try block.");
}
finally
{
    logger.LogInformation("Execution has reached the final block.");
}

Console.WriteLine("Program End.");
Console.ReadLine();
