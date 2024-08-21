using System.Text.Json.Serialization;
using LXGaming.Common.Text.Json.Serialization.Converters;

namespace LXGaming.Extractorr.Server.Services.Radarr.Models;

// https://github.com/Radarr/Radarr/blob/fc4f4ab21125cd3817133434acc0c10fba680930/src/NzbDrone.Core/MediaCover/MediaCover.cs#L6
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