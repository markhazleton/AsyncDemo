
namespace AsyncDemo.Web.Controllers.Api;

/// <summary>
/// Status Controller
/// </summary>
[Route("/status")]
public class StatusController : BaseApiController
{
    private readonly IConfiguration _config;

    /// <summary>
    /// Status Controller
    /// </summary>
    /// <param name="configuration"></param>
    public StatusController(IConfiguration configuration, IMemoryCache memoryCache) : base(memoryCache)
    {
        this._config = configuration;
    }
    /// <summary>
    /// Returns Current Application Status
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("")]
    public ApplicationStatus Get()
    {
        return GetApplicationStatus();
    }

    /// <summary>
    /// Get App Settings
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("appsettings")]
    public object GetAppSettings()
    {
        string[] TestIdList = _config.GetStringList("Async:TestIds");
        string[] MissingList = _config.GetStringList("Async:Bad");
        string[] MissingDefault = _config.GetStringList("Async:Bad", "3,2,1");
        string[] MissingDefaultSingle = _config.GetStringList("Async:Bad", "99");
        int[] TestIdIntList = _config.GetIntList("Async:TestIds");
        int[] MissingIntList = _config.GetIntList("Async:Bad");
        int[] MissingDefaultInt = _config.GetIntList("Async:Bad", "3,2,1");
        int[] BadDefaultInt = _config.GetIntList("Async:Bad", "bob,sam,1");
        return new { TestIdList, MissingList, MissingDefault, MissingDefaultSingle, TestIdIntList, MissingIntList, MissingDefaultInt, BadDefaultInt };
    }
}


