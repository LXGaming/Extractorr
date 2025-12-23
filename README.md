# Extractorr

[![License](https://img.shields.io/github/license/LXGaming/Extractorr?label=License&cacheSeconds=86400)](https://github.com/LXGaming/Extractorr/blob/main/LICENSE)
[![Docker Hub](https://img.shields.io/docker/v/lxgaming/extractorr/latest?label=Docker%20Hub)](https://hub.docker.com/r/lxgaming/extractorr)

**Extractorr** is an open source [.NET](https://dotnet.microsoft.com/) file extraction program for [Radarr](https://github.com/Radarr/Radarr) and [Sonarr](https://github.com/Sonarr/Sonarr).
Files are automatically extracted by monitoring [Flood](https://github.com/jesec/flood) or [qBittorrent](https://github.com/qbittorrent/qBittorrent) for completed torrents tagged with `extractorr`.

## Setup
### Radarr / Sonarr
1. Navigate to Settings / Connect
2. Click + (Add Connection)
3. Click Webhook 
4. Configure the Name, Webhook URL, Username and Password
5. Select the following Notification Triggers:
   - On Grab   (Used by Extractorr to tag torrents for extraction)
   - On Import (Used by Extractorr to delete extracted files)
6. Click Save

## Usage
### Docker Compose (Recommended)
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

### Docker Compose
Use environment variables
```yaml
services:
  extractorr:
    container_name: extractorr
    environment:
      - RADARR__USERNAME=sonarr
      - RADARR__PASSWORD=password
      - RADARR__DELETEONIMPORT=true
      - SONARR__USERNAME=sonarr
      - SONARR__PASSWORD=password
      - SONARR__DELETEONIMPORT=true
      - TORRENT__CLIENTS__0__TYPE=Flood
      - TORRENT__CLIENTS__0__ENABLED=true
      - TORRENT__CLIENTS__0__ADDRESS=http://flood:3000/
      - TORRENT__CLIENTS__0__USERNAME=extractorr
      - TORRENT__CLIENTS__0__PASSWORD=password
    image: lxgaming/extractorr:latest
    ports:
      - 8080:8080
    restart: unless-stopped
    volumes:
      - /path/to/downloads:/path/to/downloads
```

## License
Extractorr is licensed under the [Apache 2.0](https://github.com/LXGaming/Extractorr/blob/main/LICENSE) license.