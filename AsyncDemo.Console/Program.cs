// See https://aka.ms/new-console-template for more information
using AsyncDemo.FireAndForget;

var logger = new ConsoleLogger();
var cts = new CancellationTokenSource();
var fireAndForget = new FireAndForgetUtility(logger);

Console.WriteLine($"Program Start:  Thread:{Environment.CurrentManagedThreadId}");

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

    await Task.Delay(10000);
    cts.Cancel();
    await Task.Delay(5000);

}
catch (Exception ex)
{
    logger.TrackException(ex, "Main Exception.");
}
finally
{
    logger.TrackEvent("Main finally.");

}
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




