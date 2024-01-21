
namespace AsyncDemo.HttpGetCall;

public interface IHttpGetCallService
{
    Task<HttpGetCallResults> GetAsync<T>(HttpGetCallResults statusCall, CancellationToken ct);
}
