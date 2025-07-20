

using System;
using System.Globalization;
using System.Text.RegularExpressions;

public static class Utils
{
    public static string ToHumanReadable(Enum enumVal)
    {
        return ToHumanReadable(enumVal.ToString());
    }

    // input should be a camel case string and or has underscores
    public static string ToHumanReadable(string str)
    {
        // If string is null or empty, return the same.
        if (string.IsNullOrEmpty(str))
            return str;

        string result = Regex.Replace(str, "_", " ");

        // Insert a space before each uppercase letter that is either preceded by a lowercase letter or followed by a lowercase letter.
        result = Regex.Replace(str, "(?<=\\p{Ll})(?=\\p{Lu})|(?<=\\p{Lu})(?=\\p{Lu}\\p{Ll})", " ");

        // Convert the first letter of each word to uppercase.
        result = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(result);
        result = Regex.Replace(result, "\\s+\\band\\b", " and", RegexOptions.IgnoreCase);
        result = Regex.Replace(result, "\\s+\\bor\\b", " or", RegexOptions.IgnoreCase);
        result = Regex.Replace(result, "\\s+\\bthe\b", " the", RegexOptions.IgnoreCase);
        return result;
    }
}
