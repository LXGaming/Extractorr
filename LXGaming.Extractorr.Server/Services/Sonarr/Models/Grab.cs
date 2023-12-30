using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

public record Grab {

    [JsonPropertyName("downloadClient")]
    public string? DownloadClient { get; init; }

    [JsonPropertyName("downloadId")]
    public string? DownloadId { get; init; }

    [JsonPropertyName("episodes")]
    public List<Episode>? Episodes { get; init; }

    [JsonPropertyName("release")]
    public Release? Release { get; init; }

    [JsonPropertyName("series")]
    public Series? Series { get; init; }
}