
namespace AsyncSpark.HttpGetCall;

/// <summary>
/// Service interface for performing HTTP GET calls with deserialization support.
/// </summary>
public interface IHttpGetCallService
{
    /// <summary>
    /// Performs an asynchronous HTTP GET call and deserializes the response.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response to.</typeparam>
    /// <param name="statusCall">The status call configuration containing the URL and other settings.</param>
    /// <param name="ct">Cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the call results.</returns>
    Task<HttpGetCallResults> GetAsync<T>(HttpGetCallResults statusCall, CancellationToken ct);
}
