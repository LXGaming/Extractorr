namespace LXGaming.Extractorr.Server.Services.QBittorrent;

public sealed class QBittorrentOptions {

    public const string Key = "QBittorrent";

    public string Schedule { get; set; } = "";

    public bool RunOnStart { get; set; }
}