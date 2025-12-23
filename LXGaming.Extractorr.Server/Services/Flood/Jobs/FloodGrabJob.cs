using LXGaming.Extractorr.Server.Services.Event.Models;
using LXGaming.Extractorr.Server.Services.Quartz;
using LXGaming.Extractorr.Server.Services.Torrent;
using LXGaming.Extractorr.Server.Services.Torrent.Utilities;
using LXGaming.Extractorr.Server.Utilities;
using Quartz;

namespace LXGaming.Extractorr.Server.Services.Flood.Jobs;

public class FloodGrabJob(ILogger<FloodGrabJob> logger, TorrentService torrentService) : IJob {

    public const string EventKey = "event";
    public static readonly JobKey JobKey = JobKey.Create(nameof(FloodGrabJob));

    public async Task Execute(IJobExecutionContext context) {
        var eventArgs = context.MergedJobDataMap.GetRequired<GrabEventArgs>(EventKey);

        foreach (var torrentClient in torrentService.GetClients<FloodTorrentClient>()) {
            try {
                await ExecuteAsync(torrentClient, eventArgs);
            } catch (Exception ex) {
                logger.LogWarning(ex, "Encountered an error while executing {Client}", torrentClient);
            }
        }
    }

    private async Task ExecuteAsync(FloodTorrentClient torrentClient, GrabEventArgs eventArgs) {
        var torrentProperties = await torrentClient.GetTorrentAsync(eventArgs.Id);
        if (torrentProperties == null) {
            logger.LogWarning("Invalid Grab: {Id} was not found on {Client}", eventArgs.Id, torrentClient);
            return;
        }

        var tags = torrentProperties.Tags.Add(Constants.Application.Id);
        logger.LogDebug("Setting {Name} ({Id}) Tags: {Tags}", torrentProperties.Name, torrentProperties.Hash,
            string.Join(", ", tags));
        await torrentClient.SetTorrentTagsAsync([torrentProperties.Hash], tags);
    }
}