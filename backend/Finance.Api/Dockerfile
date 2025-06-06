# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY *.sln ./
COPY Finance.Api/*.csproj ./Finance.Api/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the code
COPY Finance.Api/. ./Finance.Api/

# Publish the app
WORKDIR /src/Finance.Api
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

# Start the app
ENTRYPOINT ["dotnet", "Finance.Api.dll"]
