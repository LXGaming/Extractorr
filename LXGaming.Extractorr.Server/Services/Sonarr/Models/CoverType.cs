using System.Text.Json.Serialization;
using LXGaming.Common.Text.Json.Serialization.Converters;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

// https://github.com/Sonarr/Sonarr/blob/1aaa9a14bc2d64cdc0d9eaac2d303b240fd2d6ea/src/NzbDrone.Core/MediaCover/MediaCover.cs#L5
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