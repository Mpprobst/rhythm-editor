using System;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

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
        result = result.ToLower();

        // Convert the first letter of each word to uppercase.
        result = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(result);
        result = Regex.Replace(result, "\\s+\\band\\b", " and", RegexOptions.IgnoreCase);
        result = Regex.Replace(result, "\\s+\\bor\\b", " or", RegexOptions.IgnoreCase);
        result = Regex.Replace(result, "\\s+\\bthe\b", " the", RegexOptions.IgnoreCase);
        return result;
    }

    // because IsSubclass doesnt returrn true if the classes are the same
    public static bool IsSubclass(Type sourceType, Type targetType)
    {
        return sourceType == targetType || sourceType.IsSubclassOf(targetType);
    }

    // could do fancy math here to calculate the enum code for the alignment, but that code is very hard to read
    // ex: any alignment that is Left, add 3 and you have the right version
    public static TMPro.TextAlignmentOptions GetTextAlignmentOption(ScreenSide screenSide, LayoutAlignment layoutAlignment)
    {
        TMPro.TextAlignmentOptions alignment = TMPro.TextAlignmentOptions.Center;
        switch (screenSide)
        {
            case ScreenSide.LEFT:
                // layoutAlignment is really top center bottom
                if (layoutAlignment == LayoutAlignment.LEFT)
                    alignment = TMPro.TextAlignmentOptions.TopLeft;
                else if (layoutAlignment == LayoutAlignment.RIGHT)
                    alignment = TMPro.TextAlignmentOptions.BottomLeft;
                else
                    alignment = TMPro.TextAlignmentOptions.Left;
                break;
            case ScreenSide.RIGHT:
                // layout alignment is really top center bottom
                if (layoutAlignment == LayoutAlignment.LEFT)
                    alignment = TMPro.TextAlignmentOptions.TopRight;
                else if (layoutAlignment == LayoutAlignment.RIGHT)
                    alignment = TMPro.TextAlignmentOptions.BottomRight;
                else
                    alignment = TMPro.TextAlignmentOptions.Right;
                break;
            case ScreenSide.TOP:
                if (layoutAlignment == LayoutAlignment.LEFT)
                    alignment = TMPro.TextAlignmentOptions.TopLeft;
                else if (layoutAlignment == LayoutAlignment.RIGHT)
                    alignment = TMPro.TextAlignmentOptions.TopRight;
                else
                    alignment = TMPro.TextAlignmentOptions.Top;
                break;
            case ScreenSide.BOTTOM:
                if (layoutAlignment == LayoutAlignment.LEFT)
                    alignment = TMPro.TextAlignmentOptions.BottomLeft;
                else if (layoutAlignment == LayoutAlignment.RIGHT)
                    alignment = TMPro.TextAlignmentOptions.BottomRight;
                else
                    alignment = TMPro.TextAlignmentOptions.Bottom;
                break;
        }

        return alignment;
    }

    public static Vector2 GetAnchorFromAlignment(ScreenSide screenSide, LayoutAlignment layoutAlignment)
    {
        var alignment = GetTextAlignmentOption(screenSide, layoutAlignment);
        string alignString = alignment.ToString();
        float x = 0;
        float y = 0;
        if (alignString.Contains("Left"))
            x = 0;
        else if (alignString.Contains("Right"))
            x = 1;
        else if (alignString.Contains("Center"))
            x = 0.5f;

        if (alignString.Contains("Bottom"))
            y = 0;
        else if (alignString.Contains("Top"))
            y = 1;
        else if (alignString.Contains("Center"))
            y = 0.5f;

        return new Vector2(x, y);
    }
}
