using System.Security.Cryptography;
using System.Text;

namespace LXGaming.Extractorr.Server.Utilities;

public static class StringUtils {

    public static bool FixedTimeEquals(string left, string right, Encoding encoding) {
        var leftBytes = encoding.GetBytes(left);
        var rightBytes = encoding.GetBytes(right);
        return CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
    }

    public static string GetEnumName(Enum @enum) {
        return Common.Utilities.StringUtils.GetEnumName(@enum);
    }

    public static Version ParseVersion(string input) {
        return input.StartsWith('v') ? Version.Parse(input[1..]) : Version.Parse(input);
    }
}