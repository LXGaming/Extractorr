﻿namespace LXGaming.Extractorr.Server.Services.Sonarr;

public class SonarrOptions {

    public const string Key = "Sonarr";

    public string? Username { get; set; }

    public string? Password { get; set; }

    public bool DeleteOnImport { get; set; }
}