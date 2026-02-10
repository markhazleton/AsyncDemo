using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AsyncSpark.HttpGetCall;

/// <summary>
/// Core implementation of HTTP GET call service with JSON deserialization.
/// Uses IHttpClientFactory for efficient HTTP client management and ILogger for structured logging.
/// </summary>
public class HttpGetCallService : IHttpGetCallService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<HttpGetCallService> _logger;

    public HttpGetCallService(ILogger<HttpGetCallService> logger, IHttpClientFactory httpClientFactory)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        _clientFactory = httpClientFactory;
        _logger = logger;
    }
    public async Task<HttpGetCallResults> GetAsync<T>(HttpGetCallResults statusCall, CancellationToken ct)
    {
        try
        {
            using var httpClient = _clientFactory.CreateClient();
            var response = await httpClient.GetAsync(statusCall.StatusPath, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var StatusResults = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
            try
            {
                statusCall.StatusResults = JsonSerializer.Deserialize<T>(StatusResults);
            }
            catch (Exception ex)
            {
                _logger.LogCritical("HttpGetCallService:GetAsync:DeserializeException: {ErrorMessage}", ex.Message);
                statusCall.StatusResults = JsonSerializer.Deserialize<dynamic>(StatusResults);
            }

        }
        catch (Exception ex)
        {
            _logger.LogCritical("HttpGetCallService:GetAsync:Exception: {ErrorMessage}", ex.Message);
        }
        return statusCall;
    }
}
