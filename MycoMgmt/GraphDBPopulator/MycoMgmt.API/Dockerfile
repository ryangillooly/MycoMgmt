FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["MycoMgmtAPI/MycoMgmtAPI.csproj", "MycoMgmtAPI/"]
RUN dotnet restore "MycoMgmtAPI/MycoMgmtAPI.csproj"
COPY . .
WORKDIR "/src/MycoMgmtAPI"
RUN dotnet build "MycoMgmtAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MycoMgmtAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MycoMgmtAPI.dll"]
