using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

// https://github.com/Sonarr/Sonarr/blob/52972e7efcce800560cbbaa64f5f76aaef6cbe77/src/NzbDrone.Core/Notifications/Webhook/WebhookEpisode.cs
public record Episode {

    [JsonPropertyName("id")]
    public int? Id { get; init; }

    [JsonPropertyName("episodeNumber")]
    public int? EpisodeNumber { get; init; }

    [JsonPropertyName("seasonNumber")]
    public int? SeasonNumber { get; init; }

    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("overview")]
    public string? Overview { get; init; }

    [JsonPropertyName("airDate")]
    public string? AirDate { get; init; }

    [JsonPropertyName("airDateUtc")]
    public DateTime? AirDateUtc { get; init; }

    [JsonPropertyName("seriesId")]
    public int? SeriesId { get; init; }

    [JsonPropertyName("tvdbId")]
    public int? TvdbId { get; init; }
}