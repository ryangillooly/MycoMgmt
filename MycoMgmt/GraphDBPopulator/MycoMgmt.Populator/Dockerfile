FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["MycoMgmt.Populator/MycoMgmt.Populator.csproj", "MycoMgmt.Populator/"]
RUN dotnet restore "MycoMgmt.Populator/MycoMgmt.Populator.csproj"
COPY . .
WORKDIR "/src/MycoMgmt.Populator"
RUN dotnet build "MycoMgmt.Populator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MycoMgmt.Populator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MycoMgmt.Populator.dll"]
