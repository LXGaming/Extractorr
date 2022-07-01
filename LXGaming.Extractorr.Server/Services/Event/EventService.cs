using LXGaming.Common.Hosting;
using LXGaming.Extractorr.Server.Services.Event.Models;

namespace LXGaming.Extractorr.Server.Services.Event;

[Service(ServiceLifetime.Singleton)]
public class EventService : IHostedService {

    public event EventHandler<GrabEventArgs>? Grab;
    public event EventHandler<ImportEventArgs>? Import;
    private readonly ILogger<EventService> _logger;

    public EventService(ILogger<EventService> logger) {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public void OnGrab(string id) {
        Task.Run(() => Grab?.Invoke(this, new GrabEventArgs {
            Id = id
        }));
    }

    public void OnImport(string id, string file, bool delete) {
        OnImport(id, new List<string> { file }, delete);
    }

    public void OnImport(string id, List<string> files, bool delete) {
        Task.Run(() => Import?.Invoke(this, new ImportEventArgs {
            Id = id,
            Files = files,
            Delete = delete
        }));
    }
}