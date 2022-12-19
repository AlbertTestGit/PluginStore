FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY PluginStore.Api.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/publish .
RUN dotnet PluginStore.Api.dll /seed
ENTRYPOINT ["dotnet", "PluginStore.Api.dll"]