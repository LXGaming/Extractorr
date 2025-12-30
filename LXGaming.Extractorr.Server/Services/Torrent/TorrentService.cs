using LXGaming.Extractorr.Server.Services.Torrent.Client;
using LXGaming.Extractorr.Server.Services.Torrent.Models;
using LXGaming.Hosting;

namespace LXGaming.Extractorr.Server.Services.Torrent;

[Service(ServiceLifetime.Singleton)]
public class TorrentService(
    IConfiguration configuration,
    ILogger<TorrentService> logger,
    IServiceProvider serviceProvider) : IHostedLifecycleService, IDisposable {

    private readonly TorrentOptions _torrentOptions = configuration.GetSection(TorrentOptions.Key).Get<TorrentOptions>()
                                                      ?? new TorrentOptions();

    private readonly HashSet<ITorrentClient> _clients = [];
    private bool _disposed;

    public Task StartingAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public Task StartAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public Task StartedAsync(CancellationToken cancellationToken) {
        var clientProviders = serviceProvider.GetServices<ITorrentClientProvider>()
            .ToDictionary(provider => provider.Type, provider => provider);
        logger.LogInformation("Discovered {Count} torrent client provider(s)", clientProviders.Count);

        foreach (var section in _torrentOptions.Clients) {
            var options = section.Get<TorrentClientOptions>();
            if (options == null) {
                logger.LogWarning("TorrentClientOptions is unavailable");
                continue;
            }

            if (options.Type == TorrentClientType.None) {
                logger.LogWarning("Type has not been configured for torrent client");
                continue;
            }

            if (!clientProviders.TryGetValue(options.Type, out var clientProvider)) {
                logger.LogWarning("{Type} torrent client provider is unavailable", options.Type);
                continue;
            }

            if (!options.Enabled) {
                logger.LogWarning("{Client} torrent client is not enabled", options);
                continue;
            }

            ITorrentClient client;
            try {
                client = clientProvider.CreateClient(section);
            } catch (Exception ex) {
                logger.LogError(ex, "Encountered an error while creating {Client} torrent client", options);
                continue;
            }

            if (_clients.Add(client)) {
                logger.LogInformation("{Client} torrent client registered", options);
            } else {
                client.Dispose();
                logger.LogWarning("{Client} torrent client is already registered", options);
            }
        }

        // Backwards Compatibility
        if (_clients.Count == 0) {
            foreach (var (clientType, clientProvider) in clientProviders) {
                ITorrentClient? client;
                try {
                    client = clientProvider.CreateLegacyClient();
                } catch (Exception ex) {
                    logger.LogError(ex, "Encountered an error while creating {Type} legacy torrent client", clientType);
                    continue;
                }

                if (client == null) {
                    continue;
                }

                if (_clients.Add(client)) {
                    logger.LogInformation("{Client} torrent client registered", client);
                } else {
                    client.Dispose();
                    logger.LogWarning("{Client} torrent client is already registered", client);
                }
            }
        }

        if (_clients.Count == 0) {
            logger.LogWarning("No torrent clients configured");
        }

        return Task.CompletedTask;
    }

    public Task StoppingAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public Task StoppedAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public ITorrentClient? GetClient(Type clientType) {
        ObjectDisposedException.ThrowIf(_disposed, this);

        return GetClients(clientType).FirstOrDefault();
    }

    public IEnumerable<ITorrentClient> GetClients(Type clientType) {
        ObjectDisposedException.ThrowIf(_disposed, this);

        return _clients.Where(clientType.IsInstanceOfType);
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        if (_disposed) {
            return;
        }

        _disposed = true;

        if (disposing) {
            foreach (var client in _clients) {
                client.Dispose();
            }
        }
    }
}