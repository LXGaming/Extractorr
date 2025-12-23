using LXGaming.Extractorr.Server.Services.Event.Models;
using LXGaming.Extractorr.Server.Services.Extraction;
using LXGaming.Extractorr.Server.Services.QBittorrent.Utilities;
using LXGaming.Extractorr.Server.Services.Quartz;
using LXGaming.Extractorr.Server.Services.Torrent;
using LXGaming.Extractorr.Server.Services.Torrent.Utilities;
using LXGaming.Extractorr.Server.Utilities;
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
        if (string.IsNullOrEmpty(torrentInfo.SavePath)) {
            logger.LogWarning("Invalid Torrent: SavePath is null or empty");
            return;
        }

        var absoluteDirectoryPath = PathUtils.GetFullDirectoryPath(torrentInfo.SavePath);
        if (!Directory.Exists(absoluteDirectoryPath)) {
            logger.LogWarning("Invalid Torrent: {Directory} does not exist", absoluteDirectoryPath);
            return;
        }

        List<string> torrentFiles;
        try {
            torrentFiles = await torrentClient.GetTorrentFilesAsync(torrentInfo);
        } catch (Exception ex) {
            logger.LogError(ex, "Encountered an error while getting torrent files for {Name} ({Id})",
                torrentInfo.Name, torrentInfo.Hash);
            return;
        }

        if (!torrentFiles.Any(extractionService.IsExtractable)) {
            logger.LogWarning("Invalid Torrent: {Id} has no extractable contents", eventArgs.Id);
            return;
        }

        foreach (var file in eventArgs.Files) {
            var absoluteFilePath = Path.GetFullPath(file);
            if (!absoluteFilePath.StartsWith(absoluteDirectoryPath)) {
                logger.LogWarning("Invalid Import File: {File}", absoluteFilePath);
                continue;
            }

            if (!File.Exists(absoluteFilePath)) {
                logger.LogWarning("Invalid Import File: {File} does not exist", absoluteFilePath);
                continue;
            }

            if (torrentFiles.Any(torrentFile => string.Equals(torrentFile, absoluteFilePath, StringComparison.OrdinalIgnoreCase))) {
                logger.LogWarning("Invalid Import File: {File} is part of the torrent", absoluteFilePath);
                continue;
            }

            logger.LogInformation("Deleting Import File: {File}", absoluteFilePath);
            File.Delete(absoluteFilePath);
        }
    }
}