FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PeakVentures.Task.PixelService/PeakVentures.Task.PixelService.csproj", "PeakVentures.Task.PixelService/"]
RUN dotnet restore "PeakVentures.Task.PixelService/PeakVentures.Task.PixelService.csproj"
COPY . .
WORKDIR "/src/PeakVentures.Task.PixelService"
RUN dotnet build "PeakVentures.Task.PixelService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PeakVentures.Task.PixelService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PeakVentures.Task.PixelService.dll"]
