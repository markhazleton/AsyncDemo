namespace AsyncSpark.Weather.Interfaces;

public interface IWeatherService
{
    Task<CurrentWeather> GetCurrentWeatherAsync(string location);
    Task<LocationForecast> GetForecastAsync(string location);
}
