using System.Text.Json.Serialization;
using LXGaming.Common.Text.Json.Serialization.Converters;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

// https://github.com/Sonarr/Sonarr/blob/52972e7efcce800560cbbaa64f5f76aaef6cbe77/src/NzbDrone.Core/Parser/Model/ReleaseType.cs
[JsonConverter(typeof(StringEnumConverter<ReleaseType>))]
public enum ReleaseType {

    [JsonPropertyName("unknown")]
    Unknown = 0,

    [JsonPropertyName("singleEpisode")]
    SingleEpisode = 1,

    [JsonPropertyName("multiEpisode")]
    MultiEpisode = 2,

    [JsonPropertyName("seasonPack")]
    SeasonPack = 3
}