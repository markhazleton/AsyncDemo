namespace AsyncSpark.Weather.Services;

/// <summary>
/// Weather service implementation using OpenWeatherMap API via WebSpark.HttpClientUtility.
/// Demonstrates how to use IHttpRequestResultService for external API calls with
/// built-in caching, telemetry, and Polly resilience provided by the library.
/// </summary>
public class OpenWeatherMapWeatherService : IWeatherService
{
    private const string BaseUrl = "http://api.openweathermap.org";
    private readonly string _apiKey;
    private readonly IHttpRequestResultService _requestService;
    private readonly ILogger<OpenWeatherMapWeatherService> _logger;

    public OpenWeatherMapWeatherService(
        string apiKey,
        IHttpRequestResultService requestService,
        ILogger<OpenWeatherMapWeatherService> logger)
    {
        _apiKey = apiKey;
        _requestService = requestService;
        _logger = logger;
    }

    public async Task<CurrentWeather> GetCurrentWeatherAsync(string location)
    {
        var url = $"{BaseUrl}/data/2.5/weather?q={location}&units=imperial&appid={_apiKey}";
        var request = new HttpRequestResult<CurrentConditionsResponse>(1, url)
        {
            CacheDurationMinutes = 90
        };

        var result = await _requestService.HttpSendRequestResultAsync(request).ConfigureAwait(false);

        if (!result.IsSuccessStatusCode || result.ResponseResults == null)
        {
            var errorMsg = result.ErrorList.Count > 0
                ? string.Join("; ", result.ErrorList)
                : $"HTTP {result.StatusCode}";
            _logger.LogError("Failed to get weather for {Location}: {Error}", location, errorMsg);
            return new CurrentWeather
            {
                ErrorMessage = errorMsg,
                Success = false
            };
        }

        _logger.LogInformation(
            "Retrieved weather for {Location} in {ElapsedMs}ms (Status: {StatusCode})",
            location, result.ElapsedMilliseconds, result.StatusCode);

        return MapCurrentConditionsResponse(result.ResponseResults);
    }

    public async Task<LocationForecast> GetForecastAsync(string location)
    {
        var url = $"{BaseUrl}/data/2.5/forecast?q={location}&units=imperial&appid={_apiKey}";
        var request = new HttpRequestResult<ForecastResponse>(1, url)
        {
            CacheDurationMinutes = 30
        };

        var result = await _requestService.HttpSendRequestResultAsync(request).ConfigureAwait(false);

        if (!result.IsSuccessStatusCode || result.ResponseResults == null)
        {
            var errorMsg = result.ErrorList.Count > 0
                ? string.Join("; ", result.ErrorList)
                : $"HTTP {result.StatusCode}";
            _logger.LogError("Failed to get forecast for {Location}: {Error}", location, errorMsg);
            return new LocationForecast
            {
                ErrorMessage = errorMsg,
                Success = false
            };
        }

        _logger.LogInformation(
            "Retrieved forecast for {Location} in {ElapsedMs}ms",
            location, result.ElapsedMilliseconds);

        return MapForecastResponse(result.ResponseResults);
    }

    private static CurrentWeather MapCurrentConditionsResponse(CurrentConditionsResponse apiResponse)
    {
        return new CurrentWeather
        {
            Success = true,
            ErrorMessage = string.Empty,
            Location = new CurrentWeather.LocationData
            {
                Name = apiResponse.Name,
                Latitude = apiResponse.Coordintates.Latitude,
                Longitude = apiResponse.Coordintates.Longitude
            },
            ObservationTime = DateTimeOffset.FromUnixTimeSeconds(apiResponse.ObservationTime + apiResponse.TimezoneOffset).DateTime,
            ObservationTimeUtc = DateTimeOffset.FromUnixTimeSeconds(apiResponse.ObservationTime).DateTime,
            CurrentConditions = new CurrentWeather.WeatherData
            {
                Conditions = apiResponse.ObservedConditions.FirstOrDefault()?.Conditions,
                ConditionsDescription = apiResponse.ObservedConditions.FirstOrDefault()?.ConditionsDetail,
                Visibility = apiResponse.Visibility / 1609.0,
                CloudCover = apiResponse.Clouds.CloudCover,
                Temperature = apiResponse.ObservationData.Temperature,
                Humidity = apiResponse.ObservationData.Humidity,
                Pressure = apiResponse.ObservationData.Pressure * 0.0295301,
                WindSpeed = apiResponse.WindData.Speed,
                WindDirection = CompassDirection.GetDirection(apiResponse.WindData.Degrees),
                WindDirectionDegrees = apiResponse.WindData.Degrees,
                RainfallOneHour = apiResponse.Rain?.RainfallOneHour ?? 0.0
            },
            FetchTime = DateTime.Now
        };
    }

    private static LocationForecast MapForecastResponse(ForecastResponse openWeatherApiResponse)
    {
        var locationForecast = new LocationForecast
        {
            Success = true,
            ErrorMessage = string.Empty,
            Location = new ForecastLocation
            {
                Name = openWeatherApiResponse.Location.Name,
                Latitude = openWeatherApiResponse.Location.Coordinates.Latitude,
                Longitude = openWeatherApiResponse.Location.Coordinates.Longitude
            },
            FetchTime = DateTime.Now
        };

        foreach (var openWeatherForecast in openWeatherApiResponse.ForecastPoints)
        {
            WeatherForecast forecast = new()
            {
                Conditions = openWeatherForecast.Conditions.FirstOrDefault()?.main,
                ConditionsDescription = openWeatherForecast.Conditions.FirstOrDefault()?.description,
                Temperature = openWeatherForecast.WeatherData.Temperature,
                Humidity = openWeatherForecast.WeatherData.Humidity,
                Pressure = openWeatherForecast.WeatherData.pressure * 0.0295301,
                ForecastTime = DateTimeOffset.FromUnixTimeSeconds(openWeatherForecast.Date + openWeatherApiResponse.Location.TimezoneOffset).DateTime,
                CloudCover = openWeatherForecast.Clouds.CloudCover,
                WindSpeed = openWeatherForecast.Wind.WindSpeed,
                WindDirectionDegrees = openWeatherForecast.Wind.WindDirectionDegrees,
                WindDirection = CompassDirection.GetDirection(openWeatherForecast.Wind.WindDirectionDegrees),
                ExpectedRainfall = (openWeatherForecast.Rain?.RainfallThreeHours ?? 0.0) * 0.03937008,
                ExpectedSnowfall = (openWeatherForecast.Snow?.SnowfallThreeHours ?? 0.0) * 0.03937008
            };
            locationForecast.Forecasts.Add(forecast);
        }

        return locationForecast;
    }
}
