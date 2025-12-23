using LXGaming.Extractorr.Server.Services.Event;
using LXGaming.Extractorr.Server.Services.Event.Models;
using LXGaming.Extractorr.Server.Services.Flood.Jobs;
using LXGaming.Extractorr.Server.Services.Torrent.Client;
using LXGaming.Extractorr.Server.Services.Torrent.Models;
using LXGaming.Hosting;
using Quartz;

namespace LXGaming.Extractorr.Server.Services.Flood;

[Service(ServiceLifetime.Singleton, typeof(ITorrentClientProvider))]
public class FloodService(
    IConfiguration configuration,
    EventService eventService,
    ILogger<FloodService> logger,
    ISchedulerFactory schedulerFactory,
    IServiceProvider serviceProvider) : IHostedService, ITorrentClientProvider {

    public TorrentClientType Type => TorrentClientType.Flood;

    private readonly FloodOptions _options = configuration.GetSection(FloodOptions.Key).Get<FloodOptions>()
                                             ?? new FloodOptions();

    public async Task StartAsync(CancellationToken cancellationToken) {
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        await scheduler.AddJob(
            JobBuilder.Create<FloodGrabJob>().WithIdentity(FloodGrabJob.JobKey).StoreDurably().Build(),
            false,
            cancellationToken);

        await scheduler.AddJob(
            JobBuilder.Create<FloodImportJob>().WithIdentity(FloodImportJob.JobKey).StoreDurably().Build(),
            false,
            cancellationToken);

        if (!string.IsNullOrEmpty(_options.Schedule)) {
            await scheduler.ScheduleJob(
                JobBuilder.Create<FloodExtractionJob>().WithIdentity(FloodExtractionJob.JobKey).Build(),
                TriggerBuilder.Create().WithCronSchedule(_options.Schedule).Build(),
                cancellationToken);
        } else {
            logger.LogWarning("Flood schedule has not been configured");
        }

        eventService.Grab += OnGrabAsync;
        eventService.Import += OnImportAsync;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public ITorrentClient CreateClient(IConfigurationSection configuration) {
        var options = configuration.Get<TorrentClientOptions>();
        if (options == null) {
            throw new InvalidOperationException("TorrentClientOptions is unavailable");
        }

        return new FloodTorrentClient(options, serviceProvider);
    }

    private async Task OnGrabAsync(object? sender, GrabEventArgs eventArgs) {
        var scheduler = await schedulerFactory.GetScheduler();
        await scheduler.TriggerJob(FloodGrabJob.JobKey, new JobDataMap {
            { FloodGrabJob.EventKey, eventArgs }
        });
    }

    private async Task OnImportAsync(object? sender, ImportEventArgs eventArgs) {
        var scheduler = await schedulerFactory.GetScheduler();
        await scheduler.TriggerJob(FloodImportJob.JobKey, new JobDataMap {
            { FloodImportJob.EventKey, eventArgs }
        });
    }
}