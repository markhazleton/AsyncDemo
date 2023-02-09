using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AsyncDemo.HttpGetCall;

public class HttpGetCallService : IHttpGetCallService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<HttpGetCallService> _logger;

    public HttpGetCallService(ILogger<HttpGetCallService> logger, IHttpClientFactory httpClientFactory)
    {
        _clientFactory = httpClientFactory;
        _logger = logger;
    }
    public async Task<HttpGetCallResults> GetAsync<T>(HttpGetCallResults statusCall)
    {
        try
        {
            using var httpClient = _clientFactory.CreateClient();
            var response = await httpClient.GetAsync(statusCall.StatusPath);
            response.EnsureSuccessStatusCode();
            var StatusResults = await response.Content.ReadAsStringAsync();
            try
            {
                statusCall.StatusResults = JsonSerializer.Deserialize<T>(StatusResults);
            }
            catch (Exception ex)
            {
                _logger.LogCritical("HttpGetCallService:GetAsync:DeserializeException", ex.Message);
                statusCall.StatusResults = JsonSerializer.Deserialize<dynamic>(StatusResults);
            }

        }
        catch (Exception ex)
        {
            _logger.LogCritical("HttpGetCallService:GetAsync:Exception", ex.Message);
        }
        return statusCall;
    }
}
