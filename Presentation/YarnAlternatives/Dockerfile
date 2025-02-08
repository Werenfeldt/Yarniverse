FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Presentation/YarnAlternatives/YarnAlternatives.csproj", "Presentation/YarnAlternatives/"]
COPY ["Infrastructure/Database/Database.csproj", "Infrastructure/Database/"]
RUN dotnet restore "Presentation/YarnAlternatives/YarnAlternatives.csproj"
COPY . .
WORKDIR "/src/Presentation/YarnAlternatives"
RUN dotnet build "YarnAlternatives.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "YarnAlternatives.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YarnAlternatives.dll"]
