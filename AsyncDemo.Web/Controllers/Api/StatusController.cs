
namespace AsyncDemo.Web.Controllers.Api;

/// <summary>
/// Status Controller
/// </summary>
[Route("/status")]
public class StatusController : BaseApiController
{
    /// <summary>
    /// Returns Current Application Status
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public ApplicationStatus Get()
    {
        return GetApplicationStatus();
    }
}
