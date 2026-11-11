
namespace DevSecOps.Dtos;

public sealed record PadelWeatherResponse(
    string Location,
    double TemperatureCelsius,
    double PrecipitationMillimeters,
    double WindSpeedKmh,
    string PlayRecommendation);