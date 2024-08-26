# Base image with ASP.NET Core runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage with .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/Obama/Obama.csproj", "Obama/"]
COPY ["src/Obama.Domain/Obama.Domain.csproj", "Obama.Domain/"]
COPY ["src/Obama.Infrastructure/Obama.Infrastructure.csproj", "Obama.Infrastructure/"]
COPY ["src/Obama.Shared/Obama.Shared.csproj", "Obama.Shared/"]

# List the contents of the directories for debugging
RUN echo "Contents of /src directory:" && ls -R /src

# Run dotnet restore with the correct path to the csproj file
RUN dotnet restore "Obama/Obama.csproj"

# Copy the entire project and build it
COPY . .
WORKDIR "/src/Obama"
RUN dotnet build "Obama.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Obama.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage with runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Obama.dll"]