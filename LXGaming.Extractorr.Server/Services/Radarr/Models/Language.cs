using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Radarr.Models;

// https://github.com/Radarr/Radarr/blob/4c007291833246d3ed78e6f396fc7e60cc9ca70c/src/NzbDrone.Core/Languages/Language.cs
public record Language {

    [JsonPropertyName("id")]
    public int? Id { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }
}