# Extractorr

[![License](https://lxgaming.github.io/badges/License-Apache%202.0-blue.svg)](https://www.apache.org/licenses/LICENSE-2.0)
[![Docker Pulls](https://img.shields.io/docker/pulls/lxgaming/extractorr)](https://hub.docker.com/r/lxgaming/extractorr)

## Docker
### Compose
```yaml
services:
  extractorr:
    environment:
      - FORWARDEDHEADERS__KNOWNPROXIES__0=127.0.0.1
      - FLOOD__ADDRESS=https://flood.example.com/
      - FLOOD__USERNAME=extractorr
      - FLOOD__PASSWORD=password
      - RADARR__USERNAME=sonarr
      - RADARR__PASSWORD=password
      - RADARR__DELETEONIMPORT=true
      - SONARR__USERNAME=sonarr
      - SONARR__PASSWORD=password
      - SONARR__DELETEONIMPORT=true
    hostname: extractorr
    image: lxgaming/extractorr:latest
    ports:
      - 80:80
```

## License
Extractorr is licensed under the [Apache 2.0](https://www.apache.org/licenses/LICENSE-2.0) license.