# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj file
COPY *.csproj ./

# Restore dependencies
RUN dotnet restore

# Copy everything else
COPY . ./

# Publish
RUN dotnet publish -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0

# Install curl for healthcheck
RUN apt-get update && apt-get install -y curl

WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Finance.Api.dll"]