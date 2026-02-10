namespace AsyncSpark.Weather.Models;

public class CurrentConditionsResponse
{
    /// <summary>
    /// A description of any error that occurred
    /// </summary>
    [JsonProperty("message")]
    public required string Message { get; set; }

    [JsonProperty("id")]
    public int CityId { get; set; }

    [JsonProperty("name")]
    public required string Name { get; set; }

    [JsonProperty("coord")]
    public required Coordinates Coordintates { get; set; }

    [JsonProperty("dt")]
    public int ObservationTime { get; set; }

    [JsonProperty("weather")]
    public required ObservedConditions[] ObservedConditions { get; set; }

    [JsonProperty("visibility")]
    public int Visibility { get; set; }

    [JsonProperty("clouds")]
    public required Clouds Clouds { get; set; }

    [JsonProperty("rain")]
    public required RainData Rain { get; set; }

    [JsonProperty("main")]
    public required ObservationData ObservationData { get; set; }

    [JsonProperty("wind")]
    public required WindData WindData { get; set; }

    [JsonProperty("sys")]
    public required LocationDetails LocationDetails { get; set; }

    [JsonProperty("timezone")]
    public int TimezoneOffset { get; set; }
}


public class Coordinates
{
    [JsonProperty("lon")]
    public float Longitude { get; set; }

    [JsonProperty("lat")]
    public float Latitude { get; set; }
}

public class ObservationData
{
    [JsonProperty("temp")]
    public float Temperature { get; set; }

    [JsonProperty("pressure")]
    public int Pressure { get; set; }

    [JsonProperty("humidity")]
    public int Humidity { get; set; }

    [JsonProperty("temp_min")]
    public float MinTemperature { get; set; }

    [JsonProperty("temp_max")]
    public float MaxTemperature { get; set; }
}

public class WindData
{
    [JsonProperty("speed")]
    public float Speed { get; set; }

    [JsonProperty("deg")]
    public int Degrees { get; set; }
}

public class Clouds
{
    [JsonProperty("all")]
    public int CloudCover { get; set; }
}

public class RainData
{
    [JsonProperty("1h")]
    public double RainfallOneHour { get; set; }
}

public class LocationDetails
{
    [JsonProperty("country")]
    public required string country { get; set; }

    [JsonProperty("sunrise")]
    public int Sunrise { get; set; }

    [JsonProperty("sunset")]
    public int Sunset { get; set; }
}

public class ObservedConditions
{
    [JsonProperty("id")]
    public int DescriptionId { get; set; }

    [JsonProperty("main")]
    public required string Conditions { get; set; }

    [JsonProperty("description")]
    public required string ConditionsDetail { get; set; }

    [JsonProperty("icon")]
    public required string Icon { get; set; }
}

