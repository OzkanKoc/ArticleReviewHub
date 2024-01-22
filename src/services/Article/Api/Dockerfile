FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["../../../../Directory.Packages.props", ""]
COPY ["src/services/Article/Api/Api.csproj", "src/services/Article/Api/"]
COPY ["src/services/Article/Infrastructure/Infrastructure.csproj", "src/services/Article/Infrastructure/"]
COPY ["src/services/Article/Domain/Domain.csproj", "src/services/Article/Domain/"]
COPY ["src/services/Article/Application/Application.csproj", "src/services/Article/Application/"]
RUN dotnet restore "src/services/Article/Api/Api.csproj"
COPY . .
WORKDIR "/src/src/services/Article/Api"
RUN dotnet build "Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]
