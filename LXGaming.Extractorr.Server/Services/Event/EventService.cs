using LXGaming.Extractorr.Server.Services.Event.Models;
using LXGaming.Hosting;

namespace LXGaming.Extractorr.Server.Services.Event;

[Service(ServiceLifetime.Singleton)]
public class EventService {

    public event AsyncEventHandler<GrabEventArgs>? Grab;
    public event AsyncEventHandler<ImportEventArgs>? Import;

    public Task OnGrabAsync(string id) {
        if (Grab == null) {
            return Task.CompletedTask;
        }

        return Grab(this, new GrabEventArgs {
            Id = id
        });
    }

    public Task OnImportAsync(string id, string file, bool delete) {
        return OnImportAsync(id, [file], delete);
    }

    public Task OnImportAsync(string id, List<string> files, bool delete) {
        if (Import == null) {
            return Task.CompletedTask;
        }

        return Import(this, new ImportEventArgs {
            Id = id,
            Files = files,
            Delete = delete
        });
    }
}