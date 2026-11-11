FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY "PadelWeather.Api.csproj" /src

RUN dotnet restore

COPY . .

RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS publish

WORKDIR /app

COPY --from=build /app/publish /app

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080
# Set the user to a non-root user for security, use 1024 > ports
USER app 

CMD ["dotnet", "PadelWeather.Api.dll"]