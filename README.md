# PadelWeather.Api

Small .NET 9 API used as the application under test for a DevSecOps exercise.

It retrieves current weather conditions from Open-Meteo and returns a simple,
illustrative recommendation for playing outdoor padel.

## Requirements

- .NET 9 SDK
- Internet access for the live weather request

## Run locally

```bash
dotnet restore
dotnet run --urls http://localhost:8080
```

## Endpoints

Health check:

```http
GET http://localhost:8080/health
```

Padel weather assessment (example coordinates for Oviedo):

```http
GET http://localhost:8080/api/weather/padel?latitude=43.3614&longitude=-5.8494&location=Oviedo
```

The weather endpoint requires latitude and longitude. The `location` parameter
is an optional display label; it is not used to geocode an address.

## Notes

- No API key or registration is required for the Open-Meteo endpoint used here.
- The recommendation thresholds are simple demonstration rules, not professional
  sports or safety advice.
- The API returns HTTP 502 if the external provider cannot be reached and HTTP 504
  if the provider times out.
- Do not add API keys, passwords, or other secrets to the repository.

## DevSecOps work to add next

1. Dockerfile and local container run.
2. GitHub Actions workflow with build and Trivy scanning.
3. Deployment to an Ubuntu Linux host.
4. Node Exporter, Prometheus and Grafana monitoring.
