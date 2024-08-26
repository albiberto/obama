#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081 

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Obama/Obama.csproj", "src/Obama/"]
COPY ["src/Obama.Domain/Obama.Domain.csproj", "src/Obama.Domain/"]
COPY ["src/Obama.Infrastructure/Obama.Infrastructure.csproj", "src/Obama.Infrastructure/"]
COPY ["src/Obama.Shared/Obama.Shared.csproj", "src/Obama.Shared/"]
RUN dotnet restore "./src/Obama/Obama.csproj"
COPY . .
WORKDIR "/src/src/Obama"
RUN dotnet build "./Obama.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Obama.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Obama.dll"]