﻿namespace AsyncDemo.Web.Controllers;
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

    private List<CurrentWeather> GetCurrentWeatherList()
    {
        List<CurrentWeather>? theList;
        if (_cache.TryGetValue<List<CurrentWeather>>("CurrentWeatherList", out theList))
        {

        }
        else
        {
            theList = new List<CurrentWeather>();
        }
        _cache.Set<List<CurrentWeather>>("CurrentWeatherList", theList ?? new List<CurrentWeather>(), TimeSpan.FromMinutes(30));
        return theList ?? new List<CurrentWeather>();
    }
    private List<CurrentWeather> AddCurrentWeatherList(CurrentWeather currentWeather)
    {
        List<CurrentWeather>? theList;
        if (_cache.TryGetValue<List<CurrentWeather>>("CurrentWeatherList", out theList))
        {
        }
        else
        {
            theList = new List<CurrentWeather>();
        }
        if (theList.Where(w => w.Location?.Name == currentWeather?.Location?.Name).Any())
        {
            // Location is already in list
        }
        else
        {
            theList.Add(currentWeather);
        }
        _cache.Set<List<CurrentWeather>>("CurrentWeatherList", theList ?? new List<CurrentWeather>(), TimeSpan.FromMinutes(30));
        return theList ?? new List<CurrentWeather>();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
    public async Task<IActionResult> Index(string? location = null)
    {
        var myList = GetCurrentWeatherList();
        string theLocation = VerifyLocation(location);

        CallEndpointMultipleTimes();

        if (myList.Where(w => w.Location?.Name == theLocation).Any())
        {
            // Location is already in list
        }
        else
        {
            CurrentWeather conditions = await _weatherService.GetCurrentWeatherAsync(theLocation);
            if (!conditions.Success)
            {
                conditions.ErrorMessage = $"{conditions.ErrorMessage} received for location:{theLocation}";
            }
            myList = AddCurrentWeatherList(conditions);
        }


        // await GetCachedWeather(myList);

        return View(myList);
    }


    public async Task CallEndpointMultipleTimes(int maxThreads = 10, int itterationCount = 100, string endpoint = "https://asyncdemoweb.azurewebsites.net/status")
    {
        // Create a SemaphoreSlim with a maximum of 10 concurrent requests
        SemaphoreSlim semaphore = new(maxThreads);

        // Create a list of tasks to make the 1000 REST calls
        List<Task> tasks = new();
        for (int i = 0; i < itterationCount; i++)
        {
            // Acquire the semaphore before making the request
            await semaphore.WaitAsync();

            // Create a task to make the request
            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    // Make the REST call
                    await GetAsync(endpoint = "https://asyncdemoweb.azurewebsites.net/status");
                }
                finally
                {
                    // Release the semaphore
                    semaphore.Release();
                }
            }));
        }

        // Wait for all tasks to complete
        await Task.WhenAll(tasks);

        // Log a message when all calls are complete
        Console.WriteLine("All calls complete");
    }

    public async Task<string> GetAsync(string url)
    {
        using var httpClient = HttpClientFactory.Create();
        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
