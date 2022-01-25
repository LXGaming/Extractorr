using LXGaming.Extractorr.Server.Utilities;
using SharpCompress.Archives;

namespace LXGaming.Extractorr.Server.Services.Extraction;

public class ExtractionService : IHostedService {

    private readonly ExtractionOptions _options;
    private readonly ILogger<ExtractionService> _logger;

    public ExtractionService(IConfiguration configuration, ILogger<ExtractionService> logger) {
        _options = configuration.GetSection(ExtractionOptions.Key).Get<ExtractionOptions>();
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public bool Execute(string path, IEnumerable<string> files) {
        var absoluteDirectoryPath = Toolbox.GetFullDirectoryPath(path);
        if (!Directory.Exists(absoluteDirectoryPath)) {
            _logger.LogWarning("Invalid Extraction: {Directory} does not exist", absoluteDirectoryPath);
            return false;
        }

        var extractableFiles = files
            .Where(File.Exists)
            .Where(IsExtractable)
            .ToList();
        if (extractableFiles.Count == 0) {
            _logger.LogInformation("No extractable files");
            return true;
        }

        var extractorrPath = Path.Join(path, $".{Constants.Application.Id}");
        if (Directory.Exists(extractorrPath)) {
            _logger.LogDebug("Cleaning up existing {Path}", extractorrPath);
            Delete(extractorrPath);
        } else {
            _logger.LogDebug("Creating {Path}", extractorrPath);
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

        _logger.LogDebug("Deleting {Path}", extractorrPath);
        Directory.Delete(extractorrPath, true);
        return true;
    }

    public bool IsExtractable(string? path) {
        return Path.HasExtension(path) && _options.Extensions.Contains(Path.GetExtension(path));
    }

    private bool Extract(string path, string destinationDirectory) {
        try {
            using var archive = ArchiveFactory.Open(path);
            _logger.LogDebug("Extracting {Path} ({ArchiveType})", path, archive.Type);
            foreach (var entry in archive.Entries) {
                if (entry.IsDirectory) {
                    continue;
                }

                var destinationFile = Path.Join(destinationDirectory, entry.Key);
                _logger.LogDebug("Extracting {Entry} -> {Destination}", entry.Key, destinationFile);
                entry.WriteToDirectory(destinationDirectory);
            }

            return true;
        } catch (Exception ex) {
            _logger.LogError(ex, "Encountered an error while extracting {Path}", path);
            return false;
        }
    }

    private bool Move(string sourceDirectory, string destinationDirectory) {
        var files = new Dictionary<string, string>();
        foreach (var file in Directory.EnumerateFiles(sourceDirectory)) {
            var destinationFile = Path.Join(destinationDirectory, Path.GetRelativePath(sourceDirectory, file));
            if (File.Exists(destinationFile)) {
                _logger.LogWarning("Cannot move {Source} as {Destination} already exists", file, destinationFile);
                return false;
            }

            files.Add(file, destinationFile);
        }

        foreach (var (key, value) in files) {
            _logger.LogDebug("Moving {Source} -> {Destination}", key, value);
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