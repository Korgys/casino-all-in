FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["casino.console/casino.console.csproj", "casino.console/"]
RUN dotnet restore "casino.console/casino.console.csproj"
COPY . .
WORKDIR "/src/casino.console"
RUN dotnet build "casino.console.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "casino.console.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "casino.console.dll"]
