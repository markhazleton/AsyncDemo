// See https://aka.ms/new-console-template for more information
using AsyncDemo;
using AsyncDemo.FireAndForget;
using Microsoft.Extensions.Logging;


var logger = new ConsoleLogger();
var adapterLogger = new LoggerAdapter<FireAndForgetUtility>(logger);
var cts = new CancellationTokenSource();
var fireAndForget = new FireAndForgetUtility(adapterLogger);

Console.WriteLine($"Program Start: Thread:{Environment.CurrentManagedThreadId}");

try
{
    // Fire off multiple tasks
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
//var asyncMock = new AsyncMockService();
//await asyncMock.ExecuteTaskAsync(true).ConfigureAwait(true);
//Console.WriteLine($"Program After Configure Await True:  Thread:{Environment.CurrentManagedThreadId}");
//await asyncMock.ExecuteTaskAsync(false).ConfigureAwait(false);
//Console.WriteLine($"Program After Configure Await False:  Thread:{Environment.CurrentManagedThreadId}");
//await asyncMock.ExecuteTaskWithTimeoutAsync(TimeSpan.FromSeconds(2));
//await asyncMock.ExecuteTaskWithTimeoutAsync(TimeSpan.FromSeconds(10));
//await asyncMock.ExecuteManuallyCancellableTaskAsync();
//await asyncMock.ExecuteManuallyCancellableTaskAsync();
//await asyncMock.CancelANonCancellableTaskAsync();




