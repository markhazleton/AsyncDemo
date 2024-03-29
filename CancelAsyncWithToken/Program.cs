﻿namespace CancelAsyncWithToken;

internal class Program
{
    private const int MillisecondsDelay = 2000;
    private static readonly CancellationTokenSource s_cts = new();

    private static readonly HttpClient s_client = new()
    {
        MaxResponseContentBufferSize = 1_000_000
    };

    private static readonly IEnumerable<string> s_urlList = new string[]
    {
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
    };

    private static async Task Main()
    {
        Console.WriteLine("Application started.");
        s_cts.CancelAfter(millisecondsDelay: MillisecondsDelay);
        try
        {
            await SumPageSizesAsync(s_cts.Token);
        }
        catch (TaskCanceledException ex)
        {
            Console.WriteLine($"Main:TaskCanceledException.{ex.Message}");
        }
        catch (OperationCanceledException ex)
        {
            Console.WriteLine($"Main:OperationCanceledException.{ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Main:Exception.{ex.Message}");
        }
        Console.WriteLine("Application ending.");
        Console.ReadKey();
    }

    private static async Task SumPageSizesAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var stopwatch = Stopwatch.StartNew();
        int total = 0;
        int docCount = 0;
        try
        {
            foreach (string url in s_urlList)
            {
                Thread.Sleep(1000);
                ct.ThrowIfCancellationRequested();

                int contentLength = await ProcessUrlAsync(url, s_client, ct);

                total += contentLength;
                docCount++;
            }
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine($"\nSumPageSizesAsync:TaskCanceledException: timed out.\n{docCount} of {s_urlList.Count()} pages counted.");
        }
        stopwatch.Stop();

        Console.WriteLine($"Total bytes returned:  {total:#,#}");
        Console.WriteLine($"Max Allowed Time: {MillisecondsDelay}");
        Console.WriteLine($"Elapsed time: {stopwatch.ElapsedMilliseconds}");
    }

    private static async Task<int> ProcessUrlAsync(string url, HttpClient client, CancellationToken token)
    {
        HttpResponseMessage response = await client.GetAsync(url, token);
        byte[] content = await response.Content.ReadAsByteArrayAsync(token).ConfigureAwait(true);
        Console.WriteLine($"{url,-60} {content.Length,10:#,#}");
        return content.Length;
    }
}
