using System.Text.Json.Serialization;
using LXGaming.Common.Text.Json.Serialization.Converters;

namespace LXGaming.Extractorr.Server.Services.Radarr.Models;

// https://github.com/Radarr/Radarr/blob/4c007291833246d3ed78e6f396fc7e60cc9ca70c/src/NzbDrone.Core/MediaCover/MediaCover.cs#L6
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