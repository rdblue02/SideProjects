#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["OuterHeavenBot.csproj", "OuterHeavenBot"]
RUN dotnet restore "OuterHeavenBot.csproj"
COPY . .
WORKDIR "/src/OuterHeavenBot"
RUN dotnet build "OuterHeavenBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OuterHeavenBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OuterHeavenBot.dll"]