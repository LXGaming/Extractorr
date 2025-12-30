using LXGaming.Extractorr.Server.Services.Flood.Models;
using LXGaming.Extractorr.Server.Utilities;

namespace LXGaming.Extractorr.Server.Services.Flood.Utilities;

public static class FloodExtensions {

    public static async Task<List<string>> GetTorrentFilesAsync(this FloodTorrentClient torrentClient,
        TorrentProperties torrentProperties) {
        var torrentPath = torrentProperties.GetPath();

        var torrentContents = await torrentClient.GetTorrentContentsAsync(torrentProperties.Hash);
        if (torrentContents == null || torrentContents.Length == 0) {
            throw new InvalidOperationException($"{torrentProperties.Name} ({torrentProperties.Hash}) has no contents");
        }

        var files = new List<string>();
        foreach (var torrentContent in torrentContents) {
            var contentPath = torrentContent.Path.TrimStart('/');
            if (string.IsNullOrEmpty(contentPath)) {
                continue;
            }

            var path = Path.GetFullPath(contentPath, torrentPath);
            if (!path.StartsWith(torrentPath)) {
                throw new IOException($"{path} is not inside {torrentPath}");
            }

            if (!File.Exists(path)) {
                if (!Directory.Exists(path)) {
                    throw new IOException($"{path} does not exist");
                }

                continue;
            }

            files.Add(path);
        }

        return files;
    }

    public static string GetPath(this TorrentProperties torrentProperties) {
        var directoryPath = PathUtils.GetFullDirectoryPath(torrentProperties.Directory);
        return Directory.Exists(directoryPath)
            ? directoryPath
            : throw new IOException($"{directoryPath} does not exist");
    }
}