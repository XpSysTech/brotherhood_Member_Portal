# ---------- BUILD ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files first (for caching)
COPY Brotherhood_Portal.API/Brotherhood_Portal.API.csproj Brotherhood_Portal.API/
COPY Brotherhood_Portal.Domain/Brotherhood_Portal.Domain.csproj Brotherhood_Portal.Domain/
COPY Brotherhood_Portal.Infrastructure/Brotherhood_Portal.Infrastructure.csproj Brotherhood_Portal.Infrastructure/
COPY Brotherhood_Portal.Application/Brotherhood_Portal.Application.csproj Brotherhood_Portal.Application/

RUN dotnet restore Brotherhood_Portal.API/Brotherhood_Portal.API.csproj

# Copy everything
COPY . .

WORKDIR /src/Brotherhood_Portal.API
RUN dotnet publish -c Release -o /app/publish

# ---------- RUNTIME ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development
# ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Brotherhood_Portal.API.dll"]