using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Radarr.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EventType {

    ApplicationUpdate,
    Download,
    Grab,
    Health,
    MovieDelete,
    MovieFileDelete,
    Rename,
    Test
}