namespace AsyncDemo.Web.Controllers;
/// <summary>
/// OpenWeather Controller for weather data management
/// </summary>
/// <remarks>
/// Initializes a new instance of the OpenWeatherController
/// </remarks>
/// <param name="logger">Logger for OpenWeatherController</param>
/// <param name="weatherService">Weather service instance</param>
/// <param name="cache">Memory cache instance</param>
/// <param name="httpClientFactory">HTTP client factory</param>
public class OpenWeatherController(ILogger<OpenWeatherController> logger, IOpenWeatherMapClient weatherService, IMemoryCache cache, IHttpClientFactory httpClientFactory) : BaseController(logger, httpClientFactory)
{
    private const string LocationCacheKey = "LocationCacheKey";

    private string VerifyLocation(string? location)
    {
        string? theLocation = location;
        if (location is null)
        {
            if (cache.TryGetValue<string>(LocationCacheKey, out theLocation))
            {
            }
            else
            {
                theLocation = "Dallas";
            }
        }
        cache.Set<string>(LocationCacheKey, theLocation ?? "Dallas", TimeSpan.FromMinutes(30));
        return theLocation ?? "Dallas";
    }

    private List<CurrentWeather> GetCurrentWeatherList()
    {
        List<CurrentWeather>? theList;
        if (cache.TryGetValue<List<CurrentWeather>>("CurrentWeatherList", out theList))
        {
        }
        else
        {
            theList = [];
        }
        cache.Set("CurrentWeatherList", theList ?? [], TimeSpan.FromMinutes(30));
        return theList ?? [];
    }
    
  private List<CurrentWeather> AddCurrentWeatherList(CurrentWeather currentWeather)
    {
   List<CurrentWeather>? theList;
        if (cache.TryGetValue<List<CurrentWeather>>("CurrentWeatherList", out theList))
        {
        }
        else
 {
         theList = [];
        }
        if (theList.Where(w => w.Location?.Name == currentWeather?.Location?.Name).Any())
        {
            // Location is already in list
    }
        else
        {
       theList.Add(currentWeather);
        }
        cache.Set<List<CurrentWeather>>("CurrentWeatherList", theList ?? [], TimeSpan.FromMinutes(30));
        return theList ?? [];
    }

    /// <summary>
    /// Index action for weather display
    /// </summary>
    /// <param name="location">Location to get weather for</param>
    /// <returns>View with weather data</returns>
    public async Task<IActionResult> Index(string? location = null)
    {
        var myList = GetCurrentWeatherList();
        string theLocation = VerifyLocation(location);

        if (myList.Where(w => w.Location?.Name == theLocation).Any())
        {
  _logger.LogInformation("Location {theLocation} is already in the list", theLocation);
        }
        else
        {
            CurrentWeather conditions = await weatherService.GetCurrentWeatherAsync(theLocation);
            if (!conditions.Success)
            {
   conditions.ErrorMessage = $"{conditions.ErrorMessage} received for location:{theLocation}";
           _logger.LogError(conditions.ErrorMessage);
  }
         myList = AddCurrentWeatherList(conditions);
     }
  return View(myList);
    }
}
