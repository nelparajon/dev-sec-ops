
namespace DevSecOps.Dtos;

public sealed record CurrentWeather(
    double Temperature,
    double Precipitation,
    double WindSpeed);