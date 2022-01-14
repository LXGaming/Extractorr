FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY *.sln .
COPY LXGaming.Extractorr.Server/*.csproj ./LXGaming.Extractorr.Server/
RUN dotnet restore

COPY LXGaming.Extractorr.Server/. ./LXGaming.Extractorr.Server/
WORKDIR /src/LXGaming.Extractorr.Server
RUN dotnet publish -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app ./
EXPOSE 80
ENTRYPOINT ["dotnet", "LXGaming.Extractorr.Server.dll"]