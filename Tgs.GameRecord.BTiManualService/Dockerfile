#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BuildConfiguration=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameRecord.BTiManualService/Tgs.GameRecord.BTiManualService.csproj", "Tgs.GameRecord.BTiManualService/"]
COPY ["Tgs.GameRecord.BTiService.Core/Tgs.GameRecord.BTiService.Core.csproj", "Tgs.GameRecord.BTiService.Core/"]
RUN dotnet restore "Tgs.GameRecord.BTiManualService/Tgs.GameRecord.BTiManualService.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecord.BTiManualService"
RUN dotnet build "Tgs.GameRecord.BTiManualService.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.GameRecord.BTiManualService.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecord.BTiManualService.dll"]