using System.Text.Json.Serialization;
using LXGaming.Common.Text.Json.Serialization.Converters;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

// https://github.com/Sonarr/Sonarr/blob/52972e7efcce800560cbbaa64f5f76aaef6cbe77/src/NzbDrone.Core/Tv/SeriesTypes.cs
[JsonConverter(typeof(StringEnumConverter<SeriesType>))]
public enum SeriesType {

    [JsonPropertyName("standard")]
    Standard = 0,

    [JsonPropertyName("daily")]
    Daily = 1,

    [JsonPropertyName("anime")]
    Anime = 2
}