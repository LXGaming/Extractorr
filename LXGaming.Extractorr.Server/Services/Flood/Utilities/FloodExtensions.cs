using LXGaming.Extractorr.Server.Services.Flood.Models;
using LXGaming.Extractorr.Server.Utilities;

namespace LXGaming.Extractorr.Server.Services.Flood.Utilities;

public static class FloodExtensions {

    public static async Task<List<string>> GetTorrentFilesAsync(this FloodTorrentClient torrentClient,
        TorrentProperties torrentProperties) {
        if (string.IsNullOrEmpty(torrentProperties.Directory) || string.IsNullOrEmpty(torrentProperties.Hash)) {
            throw new InvalidOperationException("Invalid TorrentProperties");
        }

        var absoluteDirectoryPath = PathUtils.GetFullDirectoryPath(torrentProperties.Directory);
        if (!Directory.Exists(absoluteDirectoryPath)) {
            throw new InvalidOperationException($"{absoluteDirectoryPath} does not exist");
        }

        var torrentContents = await torrentClient.GetTorrentContentsAsync(torrentProperties.Hash);
        if (torrentContents == null || torrentContents.Length == 0) {
            throw new InvalidOperationException($"{torrentProperties.Hash} has no contents");
        }

        var files = new List<string>();
        foreach (var torrentContent in torrentContents) {
            if (string.IsNullOrEmpty(torrentContent.Path)) {
                continue;
            }

            var contentPath = torrentContent.Path.TrimStart('/');
            if (string.IsNullOrEmpty(contentPath)) {
                continue;
            }

            var absolutePath = Path.GetFullPath(contentPath, absoluteDirectoryPath);
            if (!File.Exists(absolutePath)) {
                if (!Directory.Exists(absolutePath)) {
                    throw new InvalidOperationException($"{absolutePath} does not exist");
                }

                continue;
            }

            files.Add(absolutePath);
        }

        return files;
    }
}