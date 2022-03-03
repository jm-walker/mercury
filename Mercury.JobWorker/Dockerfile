#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Mercury.JobWorker/Mercury.JobWorker.csproj", "Mercury.JobWorker/"]
COPY ["Mercury.ServiceCache/Mercury.RequestCache.csproj", "Mercury.ServiceCache/"]
COPY ["mercury.common/Mercury.Common.csproj", "mercury.common/"]
COPY ["Mercury.IPlugin/Mercury.Plugin.csproj", "Mercury.IPlugin/"]
COPY ["Mercury.MessageBroker/Mercury.MessageBroker.csproj", "Mercury.MessageBroker/"]
COPY ["GeoIP/GeoIP.csproj","GeoIP/"]
RUN dotnet restore "Mercury.JobWorker/Mercury.JobWorker.csproj"
COPY . .
WORKDIR "/src/Mercury.JobWorker"
RUN dotnet build "Mercury.JobWorker.csproj" -c Release -o /app/build
WORKDIR "/src/GeoIP"
RUN dotnet build "GeoIP.csproj" -c Release -o /app/build
WORKDIR "/src/PortScan"
RUN dotnet build "PortScan.csproj" -c Release -o /app/build
WORKDIR "/src/VirusTotal"
RUN dotnet restore "VirusTotal.csproj"
RUN dotnet build "VirusTotal.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR "/src/Mercury.JobWorker"
RUN dotnet publish "Mercury.JobWorker.csproj" -c Release -o /app/publish
WORKDIR "/src/GeoIP"
RUN dotnet publish "GeoIP.csproj" -c Release -o /app/publish
WORKDIR "/src/PortScan"
RUN dotnet publish "PortScan.csproj" -c Release -o /app/publish
WORKDIR "/src/VirusTotal"
RUN dotnet publish "VirusTotal.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Mercury.JobWorker.dll"]