using Microsoft.Extensions.Logging;

namespace AsyncSpark;

/// <summary>
/// Helpers
/// </summary>
public static class Helpers
{
    public static async Task CancelANonCancellableTaskAsync(this AsyncMockService asyncMock)
    {
        System.Console.WriteLine(nameof(CancelANonCancellableTaskAsync));
        using var cancellationTokenSource = new CancellationTokenSource();
        // Listening to key press to cancel
        var keyBoardTask = Task.Run(() =>
        {
            System.Console.WriteLine("Press enter to cancel");
            System.Console.ReadKey();
            // Sending the cancellation message
            cancellationTokenSource.Cancel();
        });

        try
        {
            // Running the long running task
            var longRunningTask = asyncMock.LongRunningOperationWithCancellationTokenAsync(100,
                                                                                           cancellationTokenSource.Token).ConfigureAwait(false);
            var result = await longRunningTask;
            System.Console.WriteLine("Result {0}", result);
            System.Console.WriteLine("Press enter to continue");
        }
        catch (TaskCanceledException)
        {
            System.Console.WriteLine("Task was cancelled");
        }
        await keyBoardTask;
    }

    public static async Task ExecuteManuallyCancellableTaskAsync(this AsyncMockService asyncMock)
    {
        System.Console.WriteLine(nameof(ExecuteManuallyCancellableTaskAsync));
        using var cancellationTokenSource = new CancellationTokenSource();
        // Creating a task to listen to keyboard key press
        var keyBoardTask = Task.Run(() =>
        {
            System.Console.WriteLine("Press enter to cancel");
            System.Console.ReadKey();
            // Cancel the task
            cancellationTokenSource.Cancel();
        });

        try
        {
            var longRunningTask = AsyncMockService.LongRunningCancellableOperation(500, cancellationTokenSource.Token).ConfigureAwait(false);
            var result = await longRunningTask;
            System.Console.WriteLine("Result {0}", result);
            System.Console.WriteLine("Press enter to continue");
        }
        catch (TaskCanceledException)
        {
            System.Console.WriteLine("Task was cancelled");
        }
        await keyBoardTask;
    }

    public static async Task ExecuteTaskAsync(this AsyncMockService asyncMock, bool ConfigureAwait)
    {
        System.Console.WriteLine($"{nameof(ExecuteTaskAsync)} START ConfigureAwait:{ConfigureAwait}  Thread:{Environment.CurrentManagedThreadId}");
        System.Console.WriteLine("Result {0}", await asyncMock.LongRunningOperation(1000).ConfigureAwait(ConfigureAwait));
        System.Console.WriteLine($"{nameof(ExecuteTaskAsync)} END ConfigureAwait:{ConfigureAwait}  Thread:{Environment.CurrentManagedThreadId}\n");
    }

    public static async Task ExecuteTaskWithTimeoutAsync(this AsyncMockService asyncMock, TimeSpan timeSpan)
    {
        System.Console.WriteLine(nameof(ExecuteTaskWithTimeoutAsync));
        using (var cancellationTokenSource = new CancellationTokenSource(timeSpan))
        {
            try
            {
                var result = await AsyncMockService.LongRunningCancellableOperation(500, cancellationTokenSource.Token).ConfigureAwait(false);
                System.Console.WriteLine("Result {0}", result);
            }
            catch (TaskCanceledException)
            {
                System.Console.WriteLine("Task was cancelled");
            }
        }
        System.Console.WriteLine("Press enter to continue");
        System.Console.ReadLine();
    }
}
public class LoggerAdapter<T> : ILogger<T>
{
    private readonly ILogger _logger;

    public LoggerAdapter(ILogger logger)
    {
        _logger = logger;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => _logger.BeginScope(state);

    public bool IsEnabled(LogLevel logLevel) => _logger.IsEnabled(logLevel);

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _logger.Log(logLevel, eventId, formatter(state, exception), exception);
    }
}
