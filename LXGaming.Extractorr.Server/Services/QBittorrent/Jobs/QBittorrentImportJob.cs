using LXGaming.Extractorr.Server.Services.Event.Models;
using LXGaming.Extractorr.Server.Services.Extraction;
using LXGaming.Extractorr.Server.Services.QBittorrent.Utilities;
using LXGaming.Extractorr.Server.Services.Quartz;
using LXGaming.Extractorr.Server.Services.Torrent;
using LXGaming.Extractorr.Server.Services.Torrent.Utilities;
using Quartz;

namespace LXGaming.Extractorr.Server.Services.QBittorrent.Jobs;

public class QBittorrentImportJob(
    ExtractionService extractionService,
    ILogger<QBittorrentImportJob> logger,
    TorrentService torrentService) : IJob {

    public const string EventKey = "event";
    public static readonly JobKey JobKey = JobKey.Create(nameof(QBittorrentImportJob));

    public async Task Execute(IJobExecutionContext context) {
        var eventArgs = context.MergedJobDataMap.GetRequired<ImportEventArgs>(EventKey);

        foreach (var torrentClient in torrentService.GetClients<QBittorrentTorrentClient>()) {
            try {
                await ExecuteAsync(torrentClient, eventArgs);
            } catch (Exception ex) {
                logger.LogWarning(ex, "Encountered an error while executing {Client}", torrentClient);
            }
        }
    }

    private async Task ExecuteAsync(QBittorrentTorrentClient torrentClient, ImportEventArgs eventArgs) {
        if (!eventArgs.Delete) {
            return;
        }

        var torrentInfos = await torrentClient.GetTorrentInfosAsync(hashes: [eventArgs.Id]);
        if (torrentInfos.Length == 0) {
            logger.LogWarning("Invalid Import: {Id} was not found on {Client}", eventArgs.Id, torrentClient);
            return;
        }

        var torrentInfo = torrentInfos.Single();
        string torrentPath;
        try {
            torrentPath = torrentInfo.GetPath();
        } catch (Exception ex) {
            logger.LogWarning("Invalid Torrent: {Message}", ex.Message);
            return;
        }

        List<string> torrentFiles;
        try {
            torrentFiles = await torrentClient.GetTorrentFilesAsync(torrentInfo);
        } catch (Exception ex) {
            logger.LogError(ex, "Encountered an error while getting torrent files for {Name} ({Id})", torrentInfo.Name,
                torrentInfo.Hash);
            return;
        }

        if (!torrentFiles.Any(extractionService.IsExtractable)) {
            logger.LogWarning("Invalid Torrent: {Name} ({Id}) has no extractable contents", torrentInfo.Name,
                torrentInfo.Hash);
            return;
        }

        foreach (var file in eventArgs.Files) {
            var filePath = Path.GetFullPath(file);
            if (!filePath.StartsWith(torrentPath)) {
                logger.LogWarning("Invalid Import File: {File} is not inside {Path}", filePath, torrentPath);
                continue;
            }

            if (!File.Exists(filePath)) {
                logger.LogWarning("Invalid Import File: {File} does not exist", filePath);
                continue;
            }

            if (torrentFiles.Any(torrentFile => string.Equals(torrentFile, filePath, StringComparison.OrdinalIgnoreCase))) {
                logger.LogWarning("Invalid Import File: {File} is part of the torrent", filePath);
                continue;
            }

            logger.LogInformation("Deleting Import File: {File}", filePath);
            File.Delete(filePath);
        }
    }
}