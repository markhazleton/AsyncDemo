namespace AsyncDemo;

/// <summary>
/// Helpers
/// </summary>
public static class Helpers
{
    public static async Task CancelANonCancellableTaskAsync(this AsyncMockService asyncMock)
    {
        Console.WriteLine(nameof(CancelANonCancellableTaskAsync));
        using var cancellationTokenSource = new CancellationTokenSource();
        // Listening to key press to cancel
        var keyBoardTask = Task.Run(() =>
        {
            Console.WriteLine("Press enter to cancel");
            Console.ReadKey();
            // Sending the cancellation message
            cancellationTokenSource.Cancel();
        });

        try
        {
            // Running the long running task
            var longRunningTask = asyncMock.LongRunningOperationWithCancellationTokenAsync(100,
                                                                                           cancellationTokenSource.Token).ConfigureAwait(false);
            var result = await longRunningTask;
            Console.WriteLine("Result {0}", result);
            Console.WriteLine("Press enter to continue");
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("Task was cancelled");
        }
        await keyBoardTask;
    }

    public static async Task ExecuteManuallyCancellableTaskAsync(this AsyncMockService asyncMock)
    {
        Console.WriteLine(nameof(ExecuteManuallyCancellableTaskAsync));
        using var cancellationTokenSource = new CancellationTokenSource();
        // Creating a task to listen to keyboard key press
        var keyBoardTask = Task.Run(() =>
        {
            Console.WriteLine("Press enter to cancel");
            Console.ReadKey();
            // Cancel the task
            cancellationTokenSource.Cancel();
        });

        try
        {
            var longRunningTask = AsyncMockService.LongRunningCancellableOperation(500, cancellationTokenSource.Token).ConfigureAwait(false);
            var result = await longRunningTask;
            Console.WriteLine("Result {0}", result);
            Console.WriteLine("Press enter to continue");
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("Task was cancelled");
        }
        await keyBoardTask;
    }

    public static async Task ExecuteTaskAsync(this AsyncMockService asyncMock, bool ConfigureAwait)
    {
        Console.WriteLine($"{nameof(ExecuteTaskAsync)} START ConfigureAwait:{ConfigureAwait}  Thread:{Environment.CurrentManagedThreadId}");
        Console.WriteLine("Result {0}", await asyncMock.LongRunningOperation(1000).ConfigureAwait(ConfigureAwait));
        Console.WriteLine($"{nameof(ExecuteTaskAsync)} END ConfigureAwait:{ConfigureAwait}  Thread:{Environment.CurrentManagedThreadId}\n");
    }

    public static async Task ExecuteTaskWithTimeoutAsync(this AsyncMockService asyncMock, TimeSpan timeSpan)
    {
        Console.WriteLine(nameof(ExecuteTaskWithTimeoutAsync));
        using (var cancellationTokenSource = new CancellationTokenSource(timeSpan))
        {
            try
            {
                var result = await AsyncMockService.LongRunningCancellableOperation(500, cancellationTokenSource.Token).ConfigureAwait(false);
                Console.WriteLine("Result {0}", result);
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Task was cancelled");
            }
        }
        Console.WriteLine("Press enter to continue");
        Console.ReadLine();
    }
}
