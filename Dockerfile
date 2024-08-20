FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG TARGETARCH
WORKDIR /src

COPY *.sln ./
COPY LXGaming.Extractorr.Server/*.csproj LXGaming.Extractorr.Server/
RUN dotnet restore LXGaming.Extractorr.Server --arch $TARGETARCH

COPY LXGaming.Extractorr.Server/ LXGaming.Extractorr.Server/
RUN dotnet publish LXGaming.Extractorr.Server --arch $TARGETARCH --configuration Release --no-restore --output /app

FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-alpine
RUN apk add --no-cache --upgrade tzdata
WORKDIR /app
COPY --from=build /app ./
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["./LXGaming.Extractorr.Server"]