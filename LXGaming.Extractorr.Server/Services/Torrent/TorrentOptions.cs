namespace LXGaming.Extractorr.Server.Services.Torrent;

public sealed class TorrentOptions {

    public const string Key = "Torrent";

    public List<IConfigurationSection> Clients { get; set; } = [];
}