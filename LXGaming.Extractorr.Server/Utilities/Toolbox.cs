namespace LXGaming.Extractorr.Server.Utilities;

public static class Toolbox {

    public static string AppendEndingDirectorySeparator(string path) {
        return Path.EndsInDirectorySeparator(path) ? path : path + Path.DirectorySeparatorChar;
    }

    public static string GetFullDirectoryPath(string path) {
        var fullPath = Path.GetFullPath(path);
        return AppendEndingDirectorySeparator(fullPath);
    }

    public static string GetMappedPath(IDictionary<string, string> pathMappings, string path) {
        foreach (var pair in pathMappings) {
            var key = AppendEndingDirectorySeparator(pair.Key);
            var value = AppendEndingDirectorySeparator(pair.Value);
            if (!path.StartsWith(key)) {
                continue;
            }

            return value + path[key.Length..];
        }

        return path;
    }
}