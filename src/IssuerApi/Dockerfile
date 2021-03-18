#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["nl-covid19-coronatester-app-backend/nl-covid19-coronatester-app-backend.csproj", "nl-covid19-coronatester-app-backend/"]
RUN dotnet restore "nl-covid19-coronatester-app-backend/nl-covid19-coronatester-app-backend.csproj"
COPY . .
WORKDIR "/src/nl-covid19-coronatester-app-backend"
RUN dotnet build "nl-covid19-coronatester-app-backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "nl-covid19-coronatester-app-backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "nl-covid19-coronatester-app-backend.dll"]