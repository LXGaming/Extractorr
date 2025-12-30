using LXGaming.Extractorr.Server.Services.Torrent.Models;

namespace LXGaming.Extractorr.Server.Services.Torrent.Client;

public interface ITorrentClientProvider {

    TorrentClientType Type { get; }

    ITorrentClient CreateClient(IConfigurationSection section);

    ITorrentClient? CreateLegacyClient() {
        return null;
    }
}