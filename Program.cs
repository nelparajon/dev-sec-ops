using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<WeatherService>(client =>
{
    client.BaseAddress = new Uri("https://api.open-meteo.com/");
    client.Timeout = TimeSpan.FromSeconds(10);
});

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    service = "PadelWeather.Api"
}));

app.MapGet("/api/weather/padel", async (
    double latitude,
    double longitude,
    string? location,
    WeatherService weatherService,
    CancellationToken cancellationToken) =>
{
    if (latitude is < -90 or > 90 || longitude is < -180 or > 180)
    {
        return Results.BadRequest(new
        {
            error = "Latitude must be between -90 and 90 and longitude between -180 and 180."
        });
    }

    try
    {
        var current = await weatherService.GetCurrentWeatherAsync(
            latitude, longitude, cancellationToken);

        var recommendation = PadelRecommendation.Create(
            current.Temperature,
            current.Precipitation,
            current.WindSpeed);

        return Results.Ok(new PadelWeatherResponse(
            location ?? "Unknown",
            current.Temperature,
            current.Precipitation,
            current.WindSpeed,
            recommendation));
    }
    catch (HttpRequestException)
    {
        return Results.Problem(
            title: "Weather provider unavailable",
            detail: "The external weather service could not be reached.",
            statusCode: StatusCodes.Status502BadGateway);
    }
    catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
    {
        return Results.Problem(
            title: "Weather provider timeout",
            detail: "The external weather service took too long to respond.",
            statusCode: StatusCodes.Status504GatewayTimeout);
    }
});

app.Run();

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

public sealed class OpenMeteoResponse
{
    [JsonPropertyName("current")]
    public OpenMeteoCurrent? Current { get; set; }
}

public sealed class OpenMeteoCurrent
{
    [JsonPropertyName("temperature_2m")]
    public double Temperature { get; set; }

    [JsonPropertyName("precipitation")]
    public double Precipitation { get; set; }

    [JsonPropertyName("wind_speed_10m")]
    public double WindSpeed { get; set; }
}

public sealed record CurrentWeather(
    double Temperature,
    double Precipitation,
    double WindSpeed);

public sealed record PadelWeatherResponse(
    string Location,
    double TemperatureCelsius,
    double PrecipitationMillimeters,
    double WindSpeedKmh,
    string PlayRecommendation);

public static class PadelRecommendation
{
    // Simple illustrative rules for the exercise, not professional sports advice.
    public static string Create(double temperature, double precipitation, double windSpeed)
    {
        if (precipitation >= 1 || windSpeed >= 35)
        {
            return "NotRecommended";
        }

        if (temperature < 5 || temperature > 35 || windSpeed >= 25)
        {
            return "UseCaution";
        }

        return "Favorable";
    }
}
