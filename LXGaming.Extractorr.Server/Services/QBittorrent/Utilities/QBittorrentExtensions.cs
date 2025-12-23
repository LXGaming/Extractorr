using LXGaming.Extractorr.Server.Services.QBittorrent.Models;
using LXGaming.Extractorr.Server.Utilities;

namespace LXGaming.Extractorr.Server.Services.QBittorrent.Utilities;

public static class QBittorrentExtensions {

    public static async Task<List<string>> GetTorrentFilesAsync(this QBittorrentTorrentClient torrentClient,
        TorrentInfo torrentInfo) {
        if (string.IsNullOrEmpty(torrentInfo.Hash) || string.IsNullOrEmpty(torrentInfo.SavePath)) {
            throw new InvalidOperationException("Invalid TorrentInfo");
        }

        var absoluteDirectoryPath = PathUtils.GetFullDirectoryPath(torrentInfo.SavePath);
        if (!Directory.Exists(absoluteDirectoryPath)) {
            throw new InvalidOperationException($"{absoluteDirectoryPath} does not exist");
        }

        var torrentFiles = await torrentClient.GetTorrentFilesAsync(torrentInfo.Hash);
        if (torrentFiles == null || torrentFiles.Length == 0) {
            throw new InvalidOperationException($"{torrentInfo.Hash} has no contents");
        }

        var files = new List<string>();
        foreach (var torrentFile in torrentFiles) {
            if (string.IsNullOrEmpty(torrentFile.Name)) {
                continue;
            }

            var filePath = torrentFile.Name.TrimStart('/');
            if (string.IsNullOrEmpty(filePath)) {
                continue;
            }

            var absolutePath = Path.GetFullPath(filePath, absoluteDirectoryPath);
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

    public static async Task<bool> SetTorrentTagAsync(this QBittorrentTorrentClient torrentClient,
        TorrentInfo torrentInfo, string tag, bool state) {
        if (state) {
            if (torrentInfo.Tags.Contains(tag)) {
                return false;
            }

            await torrentClient.AddTorrentTagsAsync([torrentInfo.Hash], [tag]);
        } else {
            if (!torrentInfo.Tags.Contains(tag)) {
                return false;
            }

            await torrentClient.RemoveTorrentTagsAsync([torrentInfo.Hash], [tag]);
        }

        return true;
    }

    public static bool IsComplete(this TorrentState torrentState) {
        return torrentState
            is TorrentState.ForcedUploading
            or TorrentState.Uploading
            or TorrentState.StalledUploading
            or TorrentState.QueuedUploading
            or TorrentState.PausedUploading
            or TorrentState.StoppedUploading;
    }
}