namespace LXGaming.Extractorr.Server.Services.QBittorrent.Models;

public record PieceRange {

    public int Start { get; init; }

    public int End { get; init; }
}