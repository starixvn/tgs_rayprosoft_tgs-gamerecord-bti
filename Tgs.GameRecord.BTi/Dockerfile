#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameRecord.BTi/Tgs.GameRecord.BTi.csproj", "Tgs.GameRecord.BTi/"]
COPY ["Tgs.GameRecord.BTi.Common/Tgs.GameRecord.BTi.Common.csproj", "Tgs.GameRecord.BTi.Common/"]
COPY ["Tgs.GameRecord.BTi.Core/Tgs.GameRecord.BTi.Core.csproj", "Tgs.GameRecord.BTi.Core/"]
RUN dotnet restore "./Tgs.GameRecord.BTi/./Tgs.GameRecord.BTi.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecord.BTi"
RUN dotnet build "./Tgs.GameRecord.BTi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.GameRecord.BTi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecord.BTi.dll"]