#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AsyncSpark.Web/AsyncSpark.Web.csproj", "AsyncSpark.Web/"]
COPY ["OpenWeatherMapClient/OpenWeatherMapClient.csproj", "OpenWeatherMapClient/"]
COPY ["AsyncSpark/AsyncSpark.csproj", "AsyncSpark/"]
RUN dotnet restore "AsyncSpark.Web/AsyncSpark.Web.csproj"
COPY . .
WORKDIR "/src/AsyncSpark.Web"
RUN dotnet build "AsyncSpark.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AsyncSpark.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AsyncSpark.Web.dll"]
