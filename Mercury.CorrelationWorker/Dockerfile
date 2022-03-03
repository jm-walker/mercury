#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Mercury.CorrelationWorker/Mercury.CorrelationWorker.csproj", "Mercury.CorrelationWorker/"]
COPY ["Mercury.JobPersist/Mercury.JobPersist.csproj", "Mercury.JobPersist/"]
COPY ["mercury.common/Mercury.Common.csproj", "mercury.common/"]
COPY ["Mercury.IPlugin/Mercury.Plugin.csproj", "Mercury.IPlugin/"]
COPY ["Mercury.MessageBroker/Mercury.MessageBroker.csproj", "Mercury.MessageBroker/"]
RUN dotnet restore "Mercury.CorrelationWorker/Mercury.CorrelationWorker.csproj"
COPY . .
WORKDIR "/src/Mercury.CorrelationWorker"
RUN dotnet build "Mercury.CorrelationWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Mercury.CorrelationWorker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mercury.CorrelationWorker.dll"]