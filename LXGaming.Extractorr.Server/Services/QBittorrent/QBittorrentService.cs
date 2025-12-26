using LXGaming.Extractorr.Server.Services.Event;
using LXGaming.Extractorr.Server.Services.Event.Models;
using LXGaming.Extractorr.Server.Services.QBittorrent.Jobs;
using LXGaming.Extractorr.Server.Services.Torrent.Client;
using LXGaming.Extractorr.Server.Services.Torrent.Models;
using LXGaming.Hosting;
using Quartz;

namespace LXGaming.Extractorr.Server.Services.QBittorrent;

[Service(ServiceLifetime.Singleton, typeof(ITorrentClientProvider))]
public class QBittorrentService(
    IConfiguration configuration,
    EventService eventService,
    ILogger<QBittorrentService> logger,
    ISchedulerFactory schedulerFactory,
    IServiceProvider serviceProvider) : IHostedService, ITorrentClientProvider {

    public TorrentClientType Type => TorrentClientType.QBittorrent;

    private readonly QBittorrentOptions _options = configuration.GetSection(QBittorrentOptions.Key).Get<QBittorrentOptions>()
                                                   ?? new QBittorrentOptions();

    public async Task StartAsync(CancellationToken cancellationToken) {
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        await scheduler.AddJob(
            JobBuilder.Create<QBittorrentGrabJob>().WithIdentity(QBittorrentGrabJob.JobKey).StoreDurably().Build(),
            false,
            cancellationToken);

        await scheduler.AddJob(
            JobBuilder.Create<QBittorrentImportJob>().WithIdentity(QBittorrentImportJob.JobKey).StoreDurably().Build(),
            false,
            cancellationToken);

        if (!string.IsNullOrEmpty(_options.Schedule)) {
            await scheduler.ScheduleJob(
                JobBuilder.Create<QBittorrentExtractionJob>().WithIdentity(QBittorrentExtractionJob.JobKey).Build(),
                TriggerBuilder.Create().WithCronSchedule(_options.Schedule).Build(),
                cancellationToken);

            if (_options.RunOnStart) {
                await scheduler.TriggerJob(QBittorrentExtractionJob.JobKey, cancellationToken);
            }
        } else {
            logger.LogWarning("qBittorrent schedule has not been configured");
        }

        eventService.Grab += OnGrabAsync;
        eventService.Import += OnImportAsync;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public ITorrentClient CreateClient(IConfigurationSection section) {
        var options = section.Get<TorrentClientOptions>();
        if (options == null) {
            throw new InvalidOperationException("TorrentClientOptions is unavailable");
        }

        return new QBittorrentTorrentClient(options, serviceProvider);
    }

    private async Task OnGrabAsync(object? sender, GrabEventArgs eventArgs) {
        var scheduler = await schedulerFactory.GetScheduler();
        await scheduler.TriggerJob(QBittorrentGrabJob.JobKey, new JobDataMap {
            { QBittorrentGrabJob.EventKey, eventArgs }
        });
    }

    private async Task OnImportAsync(object? sender, ImportEventArgs eventArgs) {
        var scheduler = await schedulerFactory.GetScheduler();
        await scheduler.TriggerJob(QBittorrentImportJob.JobKey, new JobDataMap {
            { QBittorrentImportJob.EventKey, eventArgs }
        });
    }
}