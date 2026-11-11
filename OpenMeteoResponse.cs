
using System.Text.Json.Serialization;

namespace DevSecOps.Dtos;

public sealed class OpenMeteoResponse
{
    [JsonPropertyName("current")]
    public OpenMeteoCurrent? Current { get; set; }
}