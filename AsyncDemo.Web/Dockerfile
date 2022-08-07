#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AsyncDemo.Web/AsyncDemo.Web.csproj", "AsyncDemo.Web/"]
COPY ["OpenWeatherMapClient/OpenWeatherMapClient.csproj", "OpenWeatherMapClient/"]
COPY ["AsyncDemo/AsyncDemo.csproj", "AsyncDemo/"]
RUN dotnet restore "AsyncDemo.Web/AsyncDemo.Web.csproj"
COPY . .
WORKDIR "/src/AsyncDemo.Web"
RUN dotnet build "AsyncDemo.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AsyncDemo.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AsyncDemo.Web.dll"]