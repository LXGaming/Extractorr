using LXGaming.Common.Hosting;
using LXGaming.Extractorr.Server.Utilities;
using SharpCompress.Archives;

namespace LXGaming.Extractorr.Server.Services.Extraction;

[Service(ServiceLifetime.Singleton)]
public class ExtractionService(IConfiguration configuration, ILogger<ExtractionService> logger) : IHostedService {

    private readonly ExtractionOptions _options = configuration.GetSection(ExtractionOptions.Key).Get<ExtractionOptions>()
                                                  ?? throw new InvalidOperationException("ExtractionOptions is unavailable");

    public Task StartAsync(CancellationToken cancellationToken) {
        if (_options.Extensions.Count == 0) {
            logger.LogWarning("Extraction extensions have not been configured");
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public bool Execute(string path, IEnumerable<string> files) {
        var absoluteDirectoryPath = Toolbox.GetFullDirectoryPath(path);
        if (!Directory.Exists(absoluteDirectoryPath)) {
            logger.LogWarning("Invalid Extraction: {Directory} does not exist", absoluteDirectoryPath);
            return false;
        }

        var extractableFiles = files
            .Where(File.Exists)
            .Where(IsExtractable)
            .ToList();
        if (extractableFiles.Count == 0) {
            logger.LogInformation("No extractable files");
            return true;
        }

        var extractorrPath = Path.Join(path, $".{Constants.Application.Id}");
        if (Directory.Exists(extractorrPath)) {
            logger.LogDebug("Cleaning up existing {Path}", extractorrPath);
            Delete(extractorrPath);
        } else {
            logger.LogDebug("Creating {Path}", extractorrPath);
            Directory.CreateDirectory(extractorrPath);
        }

        foreach (var file in extractableFiles) {
            if (!Extract(file, extractorrPath)) {
                break;
            }

            if (!Move(extractorrPath, path)) {
                break;
            }
        }

        logger.LogDebug("Deleting {Path}", extractorrPath);
        Directory.Delete(extractorrPath, true);
        return true;
    }

    public bool IsExtractable(string? path) {
        return Path.HasExtension(path) && _options.Extensions.Contains(Path.GetExtension(path));
    }

    private bool Extract(string path, string destinationDirectory) {
        try {
            using var archive = ArchiveFactory.Open(path);
            logger.LogDebug("Extracting {Path} ({ArchiveType})", path, archive.Type);
            foreach (var entry in archive.Entries) {
                if (entry.IsDirectory) {
                    continue;
                }

                var destinationFile = Path.Join(destinationDirectory, entry.Key);
                logger.LogDebug("Extracting {Entry} -> {Destination}", entry.Key, destinationFile);
                entry.WriteToDirectory(destinationDirectory);
            }

            return true;
        } catch (Exception ex) {
            logger.LogError(ex, "Encountered an error while extracting {Path}", path);
            return false;
        }
    }

    private bool Move(string sourceDirectory, string destinationDirectory) {
        var files = new Dictionary<string, string>();
        foreach (var file in Directory.EnumerateFiles(sourceDirectory)) {
            var destinationFile = Path.Join(destinationDirectory, Path.GetRelativePath(sourceDirectory, file));
            if (File.Exists(destinationFile)) {
                logger.LogWarning("Cannot move {Source} as {Destination} already exists", file, destinationFile);
                return false;
            }

            files.Add(file, destinationFile);
        }

        foreach (var (key, value) in files) {
            logger.LogDebug("Moving {Source} -> {Destination}", key, value);
            File.Move(key, value);
        }

        return true;
    }

    private void Delete(string path) {
        foreach (var file in Directory.EnumerateFiles(path)) {
            File.Delete(file);
        }

        foreach (var directory in Directory.EnumerateDirectories(path)) {
            Directory.Delete(directory, true);
        }
    }
}