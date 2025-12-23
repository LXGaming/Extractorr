using LXGaming.Extractorr.Server.Services.QBittorrent.Models;
using LXGaming.Extractorr.Server.Utilities;

namespace LXGaming.Extractorr.Server.Services.QBittorrent.Utilities;

public static class QBittorrentExtensions {

    public static async Task<List<string>> GetTorrentFilesAsync(this QBittorrentTorrentClient torrentClient,
        TorrentInfo torrentInfo) {
        var torrentPath = torrentInfo.GetPath();

        var torrentFiles = await torrentClient.GetTorrentFilesAsync(torrentInfo.Hash);
        if (torrentFiles == null || torrentFiles.Length == 0) {
            throw new InvalidOperationException($"{torrentInfo.Name} ({torrentInfo.Hash}) has no contents");
        }

        var files = new List<string>();
        foreach (var torrentFile in torrentFiles) {
            var filePath = torrentFile.Name.TrimStart('/');
            if (string.IsNullOrEmpty(filePath)) {
                continue;
            }

            var path = Path.GetFullPath(filePath, torrentPath);
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

    public static string GetPath(this TorrentInfo torrentInfo) {
        var savePath = PathUtils.GetFullDirectoryPath(torrentInfo.SavePath);
        return Directory.Exists(savePath) ? savePath : throw new IOException($"{savePath} does not exist");
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