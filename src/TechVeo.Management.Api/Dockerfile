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
COPY ["src/TechVeo.Management.Api/TechVeo.Management.Api.csproj", "TechVeo.Management.Api/"]
COPY ["src/TechVeo.Management.Application/TechVeo.Management.Application.csproj", "TechVeo.Management.Application/"]
COPY ["src/TechVeo.Management.Contracts/TechVeo.Management.Contracts.csproj", "TechVeo.Management.Contracts/"]
COPY ["src/TechVeo.Management.Domain/TechVeo.Management.Domain.csproj", "TechVeo.Management.Domain/"]
COPY ["src/TechVeo.Management.Infra/TechVeo.Management.Infra.csproj", "TechVeo.Management.Infra/"]
RUN dotnet nuget add source --username "${GITHUB_ACTOR}" --password "${GITHUB_TOKEN}" --store-password-in-clear-text --name github "https://nuget.pkg.github.com/TechChallenge-Team/index.json"
RUN dotnet restore "./TechVeo.Management.Api/TechVeo.Management.Api.csproj"
COPY src/ .
WORKDIR /src/TechVeo.Management.Api
RUN dotnet build "./TechVeo.Management.Api.csproj" -c "$BUILD_CONFIGURATION" -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TechVeo.Management.Api.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TechVeo.Management.Api.dll"]
