#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
RUN apt-get update
WORKDIR /src
COPY ["Api/Api.fsproj", "Api/"]
COPY ["Persistence/Persistence.fsproj", "Persistence/"]
COPY ["Scaler/Scaler.fsproj", "Scaler/"]
run dotnet restore "Api/Api.fsproj"
COPY . .
WORKDIR /src/Api
RUN dotnet build "Api.fsproj" -c Release -o /app/build

FROM build AS publish
run dotnet publish "Api.fsproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]