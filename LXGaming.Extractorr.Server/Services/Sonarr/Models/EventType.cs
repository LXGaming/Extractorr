using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EventType {

    Download,
    EpisodeFileDelete,
    Grab,
    Health,
    Rename,
    SeriesDelete,
    Test
}