
using System.Text.Json.Serialization;

namespace DevSecOps.Dtos;

public sealed class OpenMeteoCurrent
{
    [JsonPropertyName("temperature_2m")]
    public double Temperature { get; set; }

    [JsonPropertyName("precipitation")]
    public double Precipitation { get; set; }

    [JsonPropertyName("wind_speed_10m")]
    public double WindSpeed { get; set; }
}