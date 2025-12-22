using LXGaming.Extractorr.Server.Services.Event.Models;
using LXGaming.Extractorr.Server.Utilities;
using Quartz;

namespace LXGaming.Extractorr.Server.Services.Flood.Jobs;

public class FloodGrabJob(FloodService floodService, ILogger<FloodGrabJob> logger) : IJob {

    public const string EventKey = "event";
    public static readonly JobKey JobKey = JobKey.Create(nameof(FloodGrabJob));

    public async Task Execute(IJobExecutionContext context) {
        if (context.MergedJobDataMap.Get(EventKey) is not GrabEventArgs eventArgs) {
            throw new InvalidOperationException("Unexpected Type");
        }

        var torrentProperties = await floodService.EnsureAuthenticatedAsync(() => floodService.GetTorrentAsync(eventArgs.Id));
        if (torrentProperties == null || string.IsNullOrEmpty(torrentProperties.Hash)) {
            logger.LogWarning("Invalid Grab: {Id} does not exist", eventArgs.Id);
            return;
        }

        var tags = torrentProperties.Tags;
        tags.Add(Constants.Application.Id);

        logger.LogDebug("Setting {Name} ({Id}) Tags: {Tags}", torrentProperties.Name, torrentProperties.Hash, string.Join(", ", tags));
        await floodService.SetTorrentTagsAsync([torrentProperties.Hash], tags);
    }
}