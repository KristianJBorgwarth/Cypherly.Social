FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY nuget.config .
RUN --mount=type=secret,id=nuget_token \
    sed -i "s|%NUGET_TOKEN%|$(cat /run/secrets/nuget_token)|g" nuget.config

COPY ["Social.API/Social.API.csproj", "Social.API/"]
COPY ["Social.Application/Social.Application.csproj", "Social.Application/"]
COPY ["Social.Domain/Social.Domain.csproj", "Social.Domain/"]
COPY ["Social.Infrastructure/Social.Infrastructure.csproj", "Social.Infrastructure/"]

RUN dotnet restore "Social.API/Social.API.csproj" --configfile nuget.config

COPY . .

WORKDIR "/src/Social.API"
RUN dotnet build "./Social.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Social.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Social.API.dll"]
