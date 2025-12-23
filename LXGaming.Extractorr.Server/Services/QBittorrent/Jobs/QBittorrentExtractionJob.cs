using LXGaming.Extractorr.Server.Services.Extraction;
using LXGaming.Extractorr.Server.Services.QBittorrent.Models;
using LXGaming.Extractorr.Server.Services.QBittorrent.Utilities;
using LXGaming.Extractorr.Server.Services.Torrent;
using LXGaming.Extractorr.Server.Services.Torrent.Utilities;
using LXGaming.Extractorr.Server.Utilities;
using Quartz;

namespace LXGaming.Extractorr.Server.Services.QBittorrent.Jobs;

[DisallowConcurrentExecution]
public class QBittorrentExtractionJob(
    ExtractionService extractionService,
    ILogger<QBittorrentExtractionJob> logger,
    TorrentService torrentService) : IJob {

    public static readonly JobKey JobKey = JobKey.Create(nameof(QBittorrentExtractionJob));

    public async Task Execute(IJobExecutionContext context) {
        foreach (var torrentClient in torrentService.GetClients<QBittorrentTorrentClient>()) {
            try {
                await ExecuteAsync(torrentClient);
            } catch (Exception ex) {
                logger.LogWarning(ex, "Encountered an error while executing {Client}", torrentClient);
            }
        }
    }

    private async Task ExecuteAsync(QBittorrentTorrentClient torrentClient) {
        var torrents = await torrentClient.GetTorrentInfosAsync(tag: Constants.Application.Id);
        if (torrents.Length == 0) {
            return;
        }

        torrentClient.ExcludedTorrents.RemoveWhere(hash => torrents.All(torrentInfo => torrentInfo.Hash != hash));

        if (torrentClient.SkipActiveExtraction) {
            var activeTorrents = await torrentClient.GetTorrentInfosAsync(TorrentFilter.Downloading);
            if (activeTorrents.Length != 0) {
                logger.LogDebug("Skipping extraction due to the following torrents:");
                foreach (var torrent in activeTorrents) {
                    logger.LogDebug("- {Name} ({Hash})", torrent.Name, torrent.Hash);
                }

                return;
            }
        }

        foreach (var torrent in torrents) {
            if (torrentClient.ExcludedTorrents.Contains(torrent.Hash)
                || !torrent.State.IsComplete()
                || !torrent.Tags.Contains(Constants.Application.Id)) {
                continue;
            }

            List<string> torrentFiles;
            try {
                torrentFiles = await torrentClient.GetTorrentFilesAsync(torrent);
            } catch (Exception ex) {
                logger.LogError(ex, "Encountered an error while getting torrent files for {Name} ({Hash})",
                    torrent.Name, torrent.Hash);
                continue;
            }

            if (torrentFiles.Any(extractionService.IsExtractable)) {
                logger.LogDebug("Processing {Name} ({Hash}) for extraction", torrent.Name, torrent.Hash);
                if (!await extractionService.ExecuteAsync(torrentFiles)) {
                    torrentClient.ExcludedTorrents.Add(torrent.Hash);
                    continue;
                }
            } else {
                logger.LogWarning("Skipping {Name} ({Hash}) due to no extractable contents", torrent.Name, torrent.Hash);
            }

            if (await torrentClient.SetTorrentTagAsync(torrent, Constants.Application.Id, false)) {
                logger.LogDebug("Removed {Name} ({Id}) Tags: {Tags}", torrent.Name, torrent.Hash,
                    Constants.Application.Id);
            }
        }
    }
}