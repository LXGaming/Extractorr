using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SeriesType {

    Standard = 0,
    Daily = 1,
    Anime = 2
}