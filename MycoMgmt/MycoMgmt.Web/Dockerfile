﻿FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["MycoMgmt.Web/MycoMgmt.Web.csproj", "MycoMgmt.Web/"]
RUN dotnet restore "MycoMgmt.Web/MycoMgmt.Web.csproj"
COPY . .
WORKDIR "/src/MycoMgmt.Web"
RUN dotnet build "MycoMgmt.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MycoMgmt.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MycoMgmt.Web.dll"]
