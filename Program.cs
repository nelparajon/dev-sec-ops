using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using DevSecOps.Dtos;
using DevSecOps.Services;

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