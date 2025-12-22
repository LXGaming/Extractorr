using System.Text.Json.Serialization;
using LXGaming.Common.Text.Json.Serialization.Converters;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

// https://github.com/Sonarr/Sonarr/blob/52972e7efcce800560cbbaa64f5f76aaef6cbe77/src/NzbDrone.Core/MediaCover/MediaCover.cs#L5
[JsonConverter(typeof(StringEnumConverter<CoverType>))]
public enum CoverType {

    [JsonPropertyName("unknown")]
    Unknown = 0,

    [JsonPropertyName("poster")]
    Poster = 1,

    [JsonPropertyName("banner")]
    Banner = 2,

    [JsonPropertyName("fanart")]
    Fanart = 3,

    [JsonPropertyName("screenshot")]
    Screenshot = 4,

    [JsonPropertyName("headshot")]
    Headshot = 5,

    [JsonPropertyName("clearlogo")]
    Clearlogo = 6
}