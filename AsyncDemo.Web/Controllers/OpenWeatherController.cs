namespace AsyncDemo.Web.Controllers;
/// <summary>
/// 
/// </summary>
public class OpenWeatherController : BaseController
{
    private const string LocationCacheKey = "LocationCacheKey";
    private readonly IMemoryCache _cache;
    private readonly ILogger<HomeController> _logger;
    private readonly IOpenWeatherMapClient _weatherService;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="weatherService"></param>
    /// <param name="cache"></param>
    public OpenWeatherController(ILogger<HomeController> logger, IOpenWeatherMapClient weatherService, IMemoryCache cache)
    {
        _logger = logger;
        _weatherService = weatherService;
        _cache = cache;
    }
    private string VerifyLocation(string? location)
    {
        string? theLocation = location;
        if (location is null)
        {
            if (_cache.TryGetValue<string>(LocationCacheKey, out theLocation))
            {
            }
            else
            {
                theLocation = "Dallas";
            }
        }
        _cache.Set<string>(LocationCacheKey, theLocation ?? "Dallas", TimeSpan.FromMinutes(30));
        return theLocation ?? "Dallas";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
    public async Task<IActionResult> Index(string? location = null)
    {
        string theLocation = VerifyLocation(location);
        CurrentWeather conditions = await _weatherService.GetCurrentWeatherAsync(theLocation);

        var myList = new List<CurrentWeather>();
        if (!conditions.Success)
        {
            conditions.ErrorMessage = $"{conditions.ErrorMessage} received for location:{theLocation}";
            myList.Add(conditions);
        }

        var field = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
        var collection = field.GetValue(_cache) as ICollection;
        var items = new List<string>();
        if (collection != null)
        {
            foreach (var item in collection)
            {
                var methodInfo = item.GetType().GetProperty("Key");
                if (methodInfo.GetValue(item).ToString().Contains("WeatherConditions::"))
                {
                    var val = methodInfo.GetValue(item);
                    items.Add(val.ToString().Replace("WeatherConditions::", String.Empty));
                }
            }
        }
        foreach (var item in items)
        {
            myList.Add(await _weatherService.GetCurrentWeatherAsync(item));
        }
        return View(myList);
    }
}
