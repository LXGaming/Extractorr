using System.Text.Json;

namespace LXGaming.Extractorr.Server.Utilities;

public static class Toolbox {

    public static readonly JsonSerializerOptions JsonSerializerOptions = new() {
        IncludeFields = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public static string GetFullDirectoryPath(string path) {
        var fullPath = Path.GetFullPath(path);
        return Path.EndsInDirectorySeparator(fullPath) ? fullPath : fullPath + Path.DirectorySeparatorChar;
    }
}