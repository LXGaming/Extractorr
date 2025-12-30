using LXGaming.Extractorr.Server.Services.Event.Models;
using LXGaming.Extractorr.Server.Services.QBittorrent.Utilities;
using LXGaming.Extractorr.Server.Services.Quartz;
using LXGaming.Extractorr.Server.Services.Torrent;
using LXGaming.Extractorr.Server.Services.Torrent.Utilities;
using LXGaming.Extractorr.Server.Utilities;
using Quartz;

namespace LXGaming.Extractorr.Server.Services.QBittorrent.Jobs;

public class QBittorrentGrabJob(ILogger<QBittorrentGrabJob> logger, TorrentService torrentService) : IJob {

    public const string EventKey = "event";
    public static readonly JobKey JobKey = JobKey.Create(nameof(QBittorrentGrabJob));

    public async Task Execute(IJobExecutionContext context) {
        var eventArgs = context.MergedJobDataMap.GetRequired<GrabEventArgs>(EventKey);

        foreach (var torrentClient in torrentService.GetClients<QBittorrentTorrentClient>()) {
            try {
                await ExecuteAsync(torrentClient, eventArgs);
            } catch (Exception ex) {
                logger.LogWarning(ex, "Encountered an error while executing {Client}", torrentClient);
            }
        }
    }

    private async Task ExecuteAsync(QBittorrentTorrentClient torrentClient, GrabEventArgs eventArgs) {
        var torrentInfos = await torrentClient.GetTorrentInfosAsync(hashes: [eventArgs.Id]);
        if (torrentInfos.Length == 0) {
            logger.LogWarning("Invalid Grab: {Id} was not found on {Client}", eventArgs.Id, torrentClient);
            return;
        }

        var torrentInfo = torrentInfos.Single();
        if (await torrentClient.SetTorrentTagAsync(torrentInfo, Constants.Application.Id, true)) {
            logger.LogDebug("Added {Name} ({Id}) Tags: {Tags}", torrentInfo.Name, torrentInfo.Hash,
                Constants.Application.Id);
        }
    }
}