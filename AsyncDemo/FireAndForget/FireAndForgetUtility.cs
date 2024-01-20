using Microsoft.Extensions.Logging;

namespace AsyncDemo.FireAndForget;

/// <summary>
/// Sealed class for Fire And Forget with Exception Handling and Logging
/// </summary>
public sealed class FireAndForgetUtility(ILogger logger)
{
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public void SafeFireAndForget(
        Task task,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(task);

        task.ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                // Log all inner exceptions if available
                if (t.Exception != null)
                {
                    foreach (var ex in t.Exception.InnerExceptions)
                    {
                        _logger.LogError(ex, "SafeFireAndForget: Unhandled exception occurred: {ExceptionMessage}", ex.Message);
                    }
                }
            }
            else if (t.IsCanceled || ct.IsCancellationRequested)
            {
                _logger.LogInformation("SafeFireAndForget: Task was cancelled.");
            }
            else if (t.IsCompleted)
            {
                // The task has completed successfully
                _logger.LogInformation("SafeFireAndForget: Task completed successfully.");
            }
            else
            {
                _logger.LogInformation("SafeFireAndForget: Task completed successfully.");
            }
        }, ct, TaskContinuationOptions.None, TaskScheduler.Default).ConfigureAwait(false);
    }
}
