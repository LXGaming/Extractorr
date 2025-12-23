using System.Net;
using LXGaming.Extractorr.Server.Services.Extraction;
using LXGaming.Extractorr.Server.Services.Flood.Models;
using LXGaming.Extractorr.Server.Services.Flood.Utilities;
using LXGaming.Extractorr.Server.Services.Torrent;
using LXGaming.Extractorr.Server.Services.Torrent.Utilities;
using LXGaming.Extractorr.Server.Utilities;
using Quartz;

namespace LXGaming.Extractorr.Server.Services.Flood.Jobs;

[DisallowConcurrentExecution]
public class FloodExtractionJob(
    ExtractionService extractionService,
    ILogger<FloodExtractionJob> logger,
    TorrentService torrentService) : IJob {

    public static readonly JobKey JobKey = JobKey.Create(nameof(FloodExtractionJob));

    public async Task Execute(IJobExecutionContext context) {
        foreach (var torrentClient in torrentService.GetClients<FloodTorrentClient>()) {
            try {
                await ExecuteAsync(torrentClient);
            } catch (Exception ex) {
                logger.LogWarning(ex, "Encountered an error while executing {Client}", torrentClient);
            }
        }
    }

    private async Task ExecuteAsync(FloodTorrentClient torrentClient) {
        TorrentListSummary torrentListSummary;
        try {
            torrentListSummary = await torrentClient.GetTorrentsAsync();
        } catch (HttpRequestException ex) {
            if (ex is not { StatusCode: HttpStatusCode.InternalServerError }) {
                throw;
            }

            logger.LogWarning("Encountered an Internal Server Error, check Flood for more details");
            return;
        }

        if (torrentListSummary.Torrents.Count == 0) {
            return;
        }

        torrentClient.ExcludedTorrents.RemoveWhere(key => !torrentListSummary.Torrents.ContainsKey(key));

        if (torrentClient.SkipActiveExtraction) {
            var activeTorrents = torrentListSummary.Torrents
                .Where(pair => pair.Value.Status.Contains(TorrentStatus.Active)
                               && !pair.Value.Status.Contains(TorrentStatus.Complete)
                               && pair.Value.Status.Contains(TorrentStatus.Downloading))
                .ToList();
            if (activeTorrents.Count != 0) {
                logger.LogDebug("Skipping extraction due to the following torrents:");
                foreach (var (key, value) in activeTorrents) {
                    logger.LogDebug("- {Name} ({Id})", value.Name, key);
                }

                return;
            }
        }

        foreach (var (key, value) in torrentListSummary.Torrents) {
            if (torrentClient.ExcludedTorrents.Contains(key)
                || !value.Status.Contains(TorrentStatus.Complete)
                || !value.Tags.Contains(Constants.Application.Id)) {
                continue;
            }

            List<string> torrentFiles;
            try {
                torrentFiles = await torrentClient.GetTorrentFilesAsync(value);
            } catch (Exception ex) {
                logger.LogError(ex, "Encountered an error while getting torrent files for {Name} ({Id})",
                    value.Name, value.Hash);
                continue;
            }

            if (torrentFiles.Any(extractionService.IsExtractable)) {
                logger.LogDebug("Processing {Name} ({Id}) for extraction", value.Name, key);
                if (!await extractionService.ExecuteAsync(torrentFiles)) {
                    torrentClient.ExcludedTorrents.Add(key);
                    continue;
                }
            } else {
                logger.LogWarning("Skipping {Name} ({Id}) due to no extractable contents", value.Name, key);
            }

            var tags = value.Tags.Remove(Constants.Application.Id);
            logger.LogDebug("Setting {Name} ({Id}) Tags: {Tags}", value.Name, key, string.Join(", ", tags));
            await torrentClient.SetTorrentTagsAsync([key], tags);
        }
    }
}