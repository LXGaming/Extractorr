﻿using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Radarr.Models;

// https://github.com/Radarr/Radarr/blob/v5.2.6.8376/src/NzbDrone.Core/Notifications/Webhook/WebhookImportPayload.cs
public record ImportPayload : Payload {

    [JsonPropertyName("movie")]
    public Movie? Movie { get; init; }

    [JsonPropertyName("remoteMovie")]
    public RemoteMovie? RemoteMovie { get; init; }

    [JsonPropertyName("movieFile")]
    public MovieFile? MovieFile { get; init; }

    [JsonPropertyName("isUpgrade")]
    public bool? IsUpgrade { get; init; }

    [JsonPropertyName("downloadClient")]
    public string? DownloadClient { get; init; }

    [JsonPropertyName("downloadClientType")]
    public string? DownloadClientType { get; init; }

    [JsonPropertyName("downloadId")]
    public string? DownloadId { get; init; }

    [JsonPropertyName("deletedFiles")]
    public List<MovieFile>? DeletedFiles { get; init; }

    [JsonPropertyName("customFormatInfo")]
    public CustomFormatInfo? CustomFormatInfo { get; init; }

    [JsonPropertyName("release")]
    public GrabbedRelease? Release { get; init; }
}