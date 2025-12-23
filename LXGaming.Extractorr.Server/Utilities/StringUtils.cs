namespace LXGaming.Extractorr.Server.Utilities;

public static class StringUtils {

    public static string GetEnumName(Enum @enum) {
        return Common.Utilities.StringUtils.GetEnumName(@enum);
    }

    public static Version ParseVersion(string input) {
        return input.StartsWith('v') ? Version.Parse(input[1..]) : Version.Parse(input);
    }

    public static bool SlowEquals(string a, string b) {
        var diff = (uint) a.Length ^ (uint) b.Length;
        for (var i = 0; i < a.Length && i < b.Length; i++) {
            diff |= (uint) (a[i] ^ b[i]);
        }

        return diff == 0;
    }
}