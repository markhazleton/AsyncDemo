using System.Diagnostics;

namespace AsyncSpark.Console.Demos;

/// <summary>
/// Demonstrates cancellation token usage with concurrent HTTP requests.
/// Shows how CancellationTokenSource.CancelAfter() enforces a timeout
/// across multiple async operations, cancelling remaining work when time expires.
/// </summary>
public static class CancellationTokenDemo
{
    private static readonly string[] UrlList =
    [
        "https://docs.microsoft.com",
        "https://docs.microsoft.com/aspnet/core",
        "https://docs.microsoft.com/azure",
        "https://docs.microsoft.com/azure/devops",
        "https://docs.microsoft.com/dotnet",
        "https://docs.microsoft.com/dynamics365",
        "https://docs.microsoft.com/education",
        "https://docs.microsoft.com/enterprise-mobility-security",
        "https://docs.microsoft.com/gaming",
        "https://docs.microsoft.com/graph",
        "https://docs.microsoft.com/microsoft-365",
        "https://docs.microsoft.com/office",
        "https://docs.microsoft.com/powershell",
        "https://docs.microsoft.com/sql",
        "https://docs.microsoft.com/surface",
        "https://docs.microsoft.com/system-center",
        "https://docs.microsoft.com/visualstudio",
        "https://docs.microsoft.com/windows",
        "https://docs.microsoft.com/xamarin"
    ];

    public static async Task RunAsync(int timeoutMilliseconds = 2000)
    {
        System.Console.WriteLine("=== Cancellation Token Demo ===");
        System.Console.WriteLine($"Fetching {UrlList.Length} URLs with a {timeoutMilliseconds}ms timeout...");
        System.Console.WriteLine();

        using var cts = new CancellationTokenSource();
        using var client = new HttpClient { MaxResponseContentBufferSize = 1_000_000 };

        cts.CancelAfter(timeoutMilliseconds);

        var stopwatch = Stopwatch.StartNew();
        int total = 0;
        int completedCount = 0;

        try
        {
            foreach (string url in UrlList)
            {
                cts.Token.ThrowIfCancellationRequested();

                var response = await client.GetAsync(url, cts.Token);
                var content = await response.Content.ReadAsByteArrayAsync(cts.Token);

                total += content.Length;
                completedCount++;
                System.Console.WriteLine($"  {url,-60} {content.Length,10:#,#}");
            }
        }
        catch (TaskCanceledException)
        {
            System.Console.WriteLine($"\n  Timeout! Completed {completedCount} of {UrlList.Length} requests before cancellation.");
        }
        catch (OperationCanceledException)
        {
            System.Console.WriteLine($"\n  Cancelled! Completed {completedCount} of {UrlList.Length} requests.");
        }

        stopwatch.Stop();
        System.Console.WriteLine();
        System.Console.WriteLine($"  Total bytes fetched:  {total:#,#}");
        System.Console.WriteLine($"  Timeout limit:        {timeoutMilliseconds}ms");
        System.Console.WriteLine($"  Elapsed time:         {stopwatch.ElapsedMilliseconds}ms");
        System.Console.WriteLine();
    }
}
