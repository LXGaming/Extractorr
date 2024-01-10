using System.Text.Json.Serialization;
using LXGaming.Common.Text.Json.Serialization.Converters;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

// https://github.com/Sonarr/Sonarr/blob/develop/src/NzbDrone.Core/Tv/SeriesTypes.cs
[JsonConverter(typeof(StringEnumConverter<SeriesType>))]
public enum SeriesType {

    [JsonPropertyName("standard")]
    Standard = 0,

    [JsonPropertyName("daily")]
    Daily = 1,

    [JsonPropertyName("anime")]
    Anime = 2
}