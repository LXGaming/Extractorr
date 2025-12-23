namespace LXGaming.Extractorr.Server.Utilities;

public static class StringUtils {

    public static bool SlowEquals(string a, string b) {
        var diff = (uint) a.Length ^ (uint) b.Length;
        for (var i = 0; i < a.Length && i < b.Length; i++) {
            diff |= (uint) (a[i] ^ b[i]);
        }

        return diff == 0;
    }
}