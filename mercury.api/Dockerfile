#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Mercury.Api/Mercury.Api.csproj", "Mercury.Api/"]
RUN dotnet restore "Mercury.Api/Mercury.Api.csproj"
COPY . .
WORKDIR "/src/Mercury.Api"
RUN dotnet build "Mercury.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Mercury.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mercury.Api.dll"]