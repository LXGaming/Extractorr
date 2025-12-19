using LXGaming.Common.Event;
using LXGaming.Extractorr.Server.Services.Event.Models;
using LXGaming.Hosting;

namespace LXGaming.Extractorr.Server.Services.Event;

[Service(ServiceLifetime.Singleton)]
public class EventService(ILogger<EventService> logger) {

    public event AsyncEventHandler<GrabEventArgs>? Grab;
    public event AsyncEventHandler<ImportEventArgs>? Import;

    public async Task OnGrabAsync(string id) {
        try {
            await Grab.InvokeAsync(this, new GrabEventArgs {
                Id = id
            });
        } catch (Exception ex) {
            logger.LogError(ex, "Encountered an error while handling grab event");
        }
    }

    public Task OnImportAsync(string id, string file, bool delete) {
        return OnImportAsync(id, [file], delete);
    }

    public async Task OnImportAsync(string id, List<string> files, bool delete) {
        try {
            await Import.InvokeAsync(this, new ImportEventArgs {
                Id = id,
                Files = files,
                Delete = delete
            });
        } catch (Exception ex) {
            logger.LogError(ex, "Encountered an error while handling import event");
        }
    }
}