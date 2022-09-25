FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim AS build
WORKDIR /src
COPY ["SolarEdgeModbusMqtt.Host/SolarEdgeModbusMqtt.Host.csproj", "SolarEdgeModbusMqtt.Host/"]
RUN dotnet restore "SolarEdgeModbusMqtt.Host/SolarEdgeModbusMqtt.Host.csproj"
COPY . .
WORKDIR "/src/SolarEdgeModbusMqtt.Host"
RUN dotnet build "SolarEdgeModbusMqtt.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SolarEdgeModbusMqtt.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SolarEdgeModbusMqtt.Host.dll"]