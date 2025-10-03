FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore
RUN dotnet publish "AdPlatforms/AdPlatforms.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "AdPlatforms.dll"]