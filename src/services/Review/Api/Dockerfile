FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["../../../../Directory.Packages.props", ""]
COPY ["src/services/Review/Api/Api.csproj", "src/services/Review/Api/"]
COPY ["src/services/Review/Infrastructure/Infrastructure.csproj", "src/services/Review/Infrastructure/"]
COPY ["src/services/Review/Domain/Domain.csproj", "src/services/Review/Domain/"]
COPY ["src/services/Review/Application/Application.csproj", "src/services/Review/Application/"]
RUN dotnet restore "src/services/Review/Api/Api.csproj"
COPY . .
WORKDIR "/src/src/services/Review/Api"
RUN dotnet build "Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]
