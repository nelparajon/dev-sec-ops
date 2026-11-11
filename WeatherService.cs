
using System.Globalization;
using DevSecOps.Dtos;

namespace DevSecOps.Services;

public sealed class WeatherService(HttpClient httpClient)
{
    public async Task<CurrentWeather> GetCurrentWeatherAsync(
        double latitude,
        double longitude,
        CancellationToken cancellationToken)
    {
        var lat = latitude.ToString(CultureInfo.InvariantCulture);
        var lon = longitude.ToString(CultureInfo.InvariantCulture);

        var url = $"v1/forecast?latitude={lat}&longitude={lon}" +
                  "&current=temperature_2m,precipitation,wind_speed_10m";

        var response = await httpClient.GetFromJsonAsync<OpenMeteoResponse>(
            url, cancellationToken);

        if (response?.Current is null)
        {
            throw new HttpRequestException("The weather provider returned an invalid response.");
        }

        return new CurrentWeather(
            response.Current.Temperature,
            response.Current.Precipitation,
            response.Current.WindSpeed);
    }
}
