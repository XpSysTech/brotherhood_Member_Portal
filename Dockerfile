# Base stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Brotherhood_Portal.API/Brotherhood_Portal.API.csproj", "Brotherhood_Portal.API/"]
COPY ["Brotherhood_Portal.Domain/Brotherhood_Portal.Domain.csproj", "Brotherhood_Portal.Domain/"]
COPY ["Brotherhood_Portal.Infrastructure/Brotherhood_Portal.Infrastructure.csproj", "Brotherhood_Portal.Infrastructure/"]
COPY ["Brotherhood_Portal.Application/Brotherhood_Portal.Application.csproj", "Brotherhood_Portal.Application/"]

RUN dotnet restore "Brotherhood_Portal.API/Brotherhood_Portal.API.csproj"

COPY . .
WORKDIR "/src/Brotherhood_Portal.API"
RUN dotnet build "Brotherhood_Portal.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Brotherhood_Portal.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Brotherhood_Portal.API.dll"]
