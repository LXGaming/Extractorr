﻿namespace LXGaming.Extractorr.Server.Services.Radarr;

public class RadarrOptions {

    public const string Key = "Radarr";

    public string? Username { get; set; }

    public string? Password { get; set; }

    public bool DeleteOnImport { get; set; }
}