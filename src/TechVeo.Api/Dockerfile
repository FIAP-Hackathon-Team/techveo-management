ARG DOTNET_RUNTIME=mcr.microsoft.com/dotnet/aspnet:8.0
ARG DOTNET_SDK=mcr.microsoft.com/dotnet/sdk:8.0

FROM ${DOTNET_RUNTIME} AS base
RUN apt-get update && \
    apt-get install wget -y
WORKDIR /app
EXPOSE 8080

FROM ${DOTNET_SDK} AS build
ARG BUILD_CONFIGURATION=Release
ARG GITHUB_TOKEN
ARG GITHUB_ACTOR
WORKDIR /src
COPY ["src/TechVeo.Api/TechVeo.Api.csproj", "TechVeo.Api/"]
COPY ["src/TechVeo.Api.Application/TechVeo.Api.Application.csproj", "TechVeo.Api.Application/"]
COPY ["src/TechVeo.Api.Contracts/TechVeo.Api.Contracts.csproj", "TechVeo.Api.Contracts/"]
COPY ["src/TechVeo.Api.Domain/TechVeo.Api.Domain.csproj", "TechVeo.Api.Domain/"]
COPY ["src/TechVeo.Api.Infra/TechVeo.Api.Infra.csproj", "TechVeo.Api.Infra/"]
# RUN dotnet nuget add source --username "${GITHUB_ACTOR}" --password "${GITHUB_TOKEN}" --store-password-in-clear-text --name github "https://nuget.pkg.github.com/TechChallenge-Team/index.json"
RUN dotnet restore "./TechVeo.Api/TechVeo.Api.csproj"
COPY src/ .
WORKDIR /src/TechVeo.Api
RUN dotnet build "./TechVeo.Api.csproj" -c "$BUILD_CONFIGURATION" -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TechVeo.Api.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TechVeo.Api.dll"]
