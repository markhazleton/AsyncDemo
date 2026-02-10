using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AsyncSpark.HttpGetCall;

/// <summary>
/// Telemetry decorator for IHttpGetCallService that adds performance tracking and timing metrics.
/// Implements the decorator pattern to add cross-cutting telemetry concerns without modifying core business logic.
/// </summary>
public class HttpGetCallServiceTelemetry : IHttpGetCallService
{
    private readonly ILogger<HttpGetCallServiceTelemetry> _logger;
    private readonly IHttpGetCallService _service;

    public HttpGetCallServiceTelemetry(ILogger<HttpGetCallServiceTelemetry> logger, IHttpGetCallService service)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(service);
        _logger = logger;
        _service = service;
    }
    public async Task<HttpGetCallResults> GetAsync<T>(HttpGetCallResults statusCall, CancellationToken ct)
    {
        Stopwatch sw = new();
        sw.Start();
        var response = new HttpGetCallResults(statusCall);
        try
        {
            response = await _service.GetAsync<T>(statusCall, ct).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Telemetry:GetAsync:Exception: {ErrorMessage}", ex.Message);
        }
        sw.Stop();
        response.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        response.CompletionDate = DateTime.Now;
        return response;
    }
}
