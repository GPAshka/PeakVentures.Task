FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PeakVentures.Task.StorageService/PeakVentures.Task.StorageService.csproj", "PeakVentures.Task.StorageService/"]
RUN dotnet restore "PeakVentures.Task.StorageService/PeakVentures.Task.StorageService.csproj"
COPY . .
WORKDIR "/src/PeakVentures.Task.StorageService"
RUN dotnet build "PeakVentures.Task.StorageService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PeakVentures.Task.StorageService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PeakVentures.Task.StorageService.dll"]
