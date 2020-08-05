#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["ngApp_netCoreApi_kestrel/ngApp_netCoreApi_kestrel.csproj", "ngApp_netCoreApi_kestrel/"]
RUN dotnet restore "ngApp_netCoreApi_kestrel/ngApp_netCoreApi_kestrel.csproj"
COPY . .
WORKDIR "/src/ngApp_netCoreApi_kestrel"
RUN dotnet build "ngApp_netCoreApi_kestrel.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ngApp_netCoreApi_kestrel.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ngApp_netCoreApi_kestrel.dll"]