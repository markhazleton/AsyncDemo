
namespace AsyncSpark.Weather.Models;
#pragma warning disable IDE1006 // Naming Styles

public class ForecastResponse
{
    public required string cod { get; set; }
    public float message { get; set; }

    [JsonPropertyName("list")]
    public required OpenWeatherForecastData[] ForecastPoints { get; set; }

    [JsonPropertyName("city")]
    public required OpenWeatherForecastLocation Location { get; set; }
}


public class OpenWeatherForecastData
{
    [JsonPropertyName("dt")]
    public int Date { get; set; }

    [JsonPropertyName("main")]
    public required OpenWeatherForecastWeatherData WeatherData { get; set; }

    [JsonPropertyName("weather")]
    public required OpenWeatherForecastConditions[] Conditions { get; set; }

    [JsonPropertyName("clouds")]
    public required OpenWeatherForecastClouds Clouds { get; set; }

    [JsonPropertyName("wind")]
    public required OpenWeatherForecastWind Wind { get; set; }
    public required string dt_txt { get; set; }

    /// <summary>
    /// Rain data (only present when rain is forecasted)
    /// </summary>
    [JsonPropertyName("rain")]
    public OpenWeatherForecastRain? Rain { get; set; }

    /// <summary>
    /// Snow data (only present when snow is forecasted)
    /// </summary>
    [JsonPropertyName("snow")]
    public OpenWeatherForecastSnow? Snow { get; set; }
}


public class OpenWeatherForecastLocation
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("coord")]
    public required OpenWeatherForecastCoordinates Coordinates { get; set; }

    [JsonPropertyName("country")]
    public required string Country { get; set; }

    [JsonPropertyName("timezone")]
    public int TimezoneOffset { get; set; }
}

public class OpenWeatherForecastCoordinates
{
    [JsonPropertyName("lat")]
    public float Latitude { get; set; }

    [JsonPropertyName("lon")]
    public float Longitude { get; set; }
}



public class OpenWeatherForecastWeatherData
{
    [JsonPropertyName("temp")]
    public float Temperature { get; set; }
    public float temp_min { get; set; }
    public float temp_max { get; set; }

    [JsonPropertyName("pressure")]
    public float pressure { get; set; }
    public float sea_level { get; set; }
    public float grnd_level { get; set; }

    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }
}

public class OpenWeatherForecastClouds
{
    [JsonPropertyName("all")]
    public int CloudCover { get; set; }
}

public class OpenWeatherForecastWind
{
    [JsonPropertyName("speed")]
    public float WindSpeed { get; set; }

    [JsonPropertyName("deg")]
    public int WindDirectionDegrees { get; set; }
}


public class OpenWeatherForecastRain
{
    [JsonPropertyName("3h")]
    public float RainfallThreeHours { get; set; }
}

public class OpenWeatherForecastSnow
{
    [JsonPropertyName("3h")]
    public float SnowfallThreeHours { get; set; }
}

public class OpenWeatherForecastConditions
{
    public int id { get; set; }
    public required string main { get; set; }
    public required string description { get; set; }
    public required string icon { get; set; }
}
#pragma warning restore IDE1006 // Naming Styles
