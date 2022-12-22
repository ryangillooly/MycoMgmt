FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MycoMgmt/MycoMgmt.csproj", "MycoMgmt/"]
RUN dotnet restore "MycoMgmt/MycoMgmt.csproj"
COPY . .
WORKDIR "/src/MycoMgmt"
RUN dotnet build "MycoMgmt.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MycoMgmt.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MycoMgmt.dll"]
