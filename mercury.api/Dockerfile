#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["mercury.api/Mercury.Api.csproj", "mercury.api/"]
COPY ["Mercury.IPlugin/Mercury.Plugin.csproj", "Mercury.IPlugin/"]
COPY ["Mercury.JobPersist/Mercury.JobPersist.csproj", "Mercury.JobPersist/"]
COPY ["mercury.common/Mercury.Common.csproj", "mercury.common/"]
COPY ["Mercury.MessageBroker/Mercury.MessageBroker.csproj", "Mercury.MessageBroker/"]
RUN dotnet restore "mercury.api/Mercury.Api.csproj"
COPY . .
WORKDIR "/src/mercury.api"
RUN dotnet build "Mercury.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Mercury.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mercury.Api.dll"]