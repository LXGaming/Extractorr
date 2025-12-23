using System.Collections.Immutable;
using LXGaming.Extractorr.Server.Services.Extraction.Results;
using LXGaming.Extractorr.Server.Utilities;
using LXGaming.Hosting;
using SharpCompress.Archives;

namespace LXGaming.Extractorr.Server.Services.Extraction;

[Service(ServiceLifetime.Singleton)]
public class ExtractionService(IConfiguration configuration, ILogger<ExtractionService> logger) : IHostedService {

    private readonly ExtractionOptions _options = configuration.GetSection(ExtractionOptions.Key).Get<ExtractionOptions>()
                                                  ?? new ExtractionOptions();

    public Task StartAsync(CancellationToken cancellationToken) {
        if (_options.Extensions.Count == 0) {
            logger.LogWarning("Extraction extensions have not been configured");
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public async Task<bool> ExecuteAsync(IEnumerable<string> files) {
        var extractableFiles = files
            .Select(Path.GetFullPath)
            .Where(File.Exists)
            .Where(IsExtractable)
            .ToList();
        if (extractableFiles.Count == 0) {
            logger.LogInformation("No extractable files");
            return true;
        }

        while (extractableFiles.Count > 0) {
            var extractableFile = extractableFiles[0];
            extractableFiles.RemoveAt(0);

            var directoryPath = Path.GetDirectoryName(extractableFile);
            if (!Directory.Exists(directoryPath)) {
                logger.LogWarning("Cannot extract {File} as {Directory} does not exist", extractableFile,
                    directoryPath);
                continue;
            }

            var extractPath = PathUtils.GetFullDirectoryPath($"{extractableFile}.{Constants.Application.Id}");
            if (Directory.Exists(extractPath)) {
                logger.LogDebug("Cleaning up existing extraction directory {Path}", extractPath);
                Delete(extractPath, true);
            } else {
                logger.LogDebug("Creating extraction directory {Path}", extractPath);
                Directory.CreateDirectory(extractPath);
            }

            var extractResult = await ExtractAsync(extractableFile, extractPath);
            if (extractResult.Volumes != null) {
                extractableFiles.RemoveAll(extractResult.Volumes.Contains);
            }

            if (!extractResult.IsSuccess) {
                continue;
            }

            Move(extractPath, directoryPath);

            logger.LogDebug("Deleting extraction directory {Path}", extractPath);
            Directory.Delete(extractPath, true);
        }

        return true;
    }

    public bool IsExtractable(string? path) {
        return Path.HasExtension(path) && _options.Extensions.Contains(Path.GetExtension(path));
    }

    private async Task<ExtractResult> ExtractAsync(string path, string destinationDirectory) {
        var volumesBuilder = ImmutableHashSet.CreateBuilder<string>();
        try {
            using var archive = ArchiveFactory.Open(path);
            foreach (var volume in archive.Volumes) {
                if (string.IsNullOrEmpty(volume.FileName)) {
                    continue;
                }

                var fileName = Path.GetFullPath(volume.FileName);
                volumesBuilder.Add(fileName);
            }

            logger.LogInformation("Extracting {Path} ({ArchiveType})", path, archive.Type);
            foreach (var entry in archive.Entries) {
                if (entry.IsDirectory) {
                    continue;
                }

                var destinationFile = Path.Join(destinationDirectory, entry.Key);
                logger.LogDebug("Extracting {Entry} to {Destination}", entry.Key, destinationFile);
                await entry.WriteToDirectoryAsync(destinationDirectory);
            }

            return ExtractResult.FromSuccess(volumesBuilder.ToImmutable());
        } catch (Exception ex) {
            logger.LogError(ex, "Encountered an error while extracting {Path}", path);
            return ExtractResult.FromError(ex, volumesBuilder.ToImmutable());
        }
    }

    private bool Move(string sourceDirectory, string destinationDirectory) {
        var files = new Dictionary<string, string>();
        foreach (var file in Directory.EnumerateFiles(sourceDirectory, "*", SearchOption.AllDirectories)) {
            var destinationFile = Path.Combine(destinationDirectory, Path.GetRelativePath(sourceDirectory, file));
            if (File.Exists(destinationFile)) {
                logger.LogWarning("Cannot move {Source} as {Destination} already exists", file, destinationFile);
                return false;
            }

            files.Add(file, destinationFile);
        }

        foreach (var (key, value) in files) {
            logger.LogDebug("Moving {Source} to {Destination}", key, value);
            File.Move(key, value);
        }

        return true;
    }

    private static void Delete(string path, bool recursive) {
        foreach (var file in Directory.EnumerateFiles(path)) {
            File.Delete(file);
        }

        foreach (var directory in Directory.EnumerateDirectories(path)) {
            Directory.Delete(directory, recursive);
        }
    }
}