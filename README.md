# Extractorr

[![License](https://img.shields.io/github/license/LXGaming/Extractorr?label=License&cacheSeconds=86400)](https://github.com/LXGaming/Extractorr/blob/main/LICENSE)
[![Docker Hub](https://img.shields.io/docker/v/lxgaming/extractorr/latest?label=Docker%20Hub)](https://hub.docker.com/r/lxgaming/extractorr)

Extractorr is a file extraction program for [Radarr](https://github.com/Radarr/Radarr) and [Sonarr](https://github.com/Sonarr/Sonarr).
It extracts files by monitoring [Flood](https://github.com/jesec/flood) for completed torrents tagged with `extractorr` 

## Setup
### Radarr / Sonarr
Setting -> Connect -> + -> Webhook\
Triggers:
- On Grab: Torrent is tagged with `extractorr`
- On Import: Torrent file is optionally deleted

## Usage
### docker-compose (Recommended)
Download and use [appsettings.json](https://raw.githubusercontent.com/LXGaming/Extractorr/main/LXGaming.Extractorr.Server/appsettings.json)
```yaml
services:
  extractorr:
    container_name: extractorr
    image: lxgaming/extractorr:latest
    ports:
     - 8080:8080
    restart: unless-stopped
    volumes:
      - /path/to/downloads:/path/to/downloads
      - /path/to/extractorr/logs:/app/logs
      - /path/to/extractorr/appsettings.json:/app/appsettings.json:ro
```

### docker-compose
Use environment variables
```yaml
services:
  extractorr:
    container_name: extractorr
    environment:
      - FLOOD__ADDRESS=https://flood.example.com/
      - FLOOD__USERNAME=extractorr
      - FLOOD__PASSWORD=password
      - RADARR__USERNAME=sonarr
      - RADARR__PASSWORD=password
      - RADARR__DELETEONIMPORT=true
      - SONARR__USERNAME=sonarr
      - SONARR__PASSWORD=password
      - SONARR__DELETEONIMPORT=true
    image: lxgaming/extractorr:latest
    ports:
     - 8080:8080
    restart: unless-stopped
    volumes:
      - /path/to/downloads:/path/to/downloads
```

## License
Extractorr is licensed under the [Apache 2.0](https://github.com/LXGaming/Extractorr/blob/main/LICENSE) license.