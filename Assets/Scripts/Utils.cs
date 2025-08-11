using System;
using System.Linq;
using System.Globalization;
using System.Runtime.InteropServices.WindowsRuntime;
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

    public static ScreenSide ScreenSideOfElement(RectTransform rxForm)
    {
        // whichever axis is closer to an extent we should use
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Canvas canvas = rxForm.GetComponentInParent<Canvas>();
        if (canvas)
            screenSize = canvas.renderingDisplaySize;   // because this is how transforms determine their position

        ScreenSide screenSide = ScreenSide.LEFT;
        Vector2 posDiff = new Vector2(rxForm.position.x, rxForm.position.y) - screenSize / 2f;    // using center of screen because negative values clearly indicate our quadrant of screen
        posDiff = new Vector2(posDiff.x / screenSize.x, posDiff.y / screenSize.y);  // scales so our comparison between axes is fair
        if (Mathf.Abs(posDiff.x) < Mathf.Abs(posDiff.y))
        {
            // we are closer to a top or bottom
            if (posDiff.y < 0) screenSide = ScreenSide.BOTTOM;
            else screenSide = ScreenSide.TOP;
        }
        else
        {
            if (posDiff.x > 0)
                screenSide = ScreenSide.RIGHT;
        }
        return screenSide;
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

        if (alignString.Contains("Bottom"))
            y = 0;
        else if (alignString.Contains("Top"))
            y = 1;
        else
            y = 0.5f;

        if (alignString.Contains("Left"))
            x = 0;
        else if (alignString.Contains("Right") && y == 0)   // checking y == 0 because it looks better to be flush to edges
            x = 1;
        else
            x = 0.5f;

        return new Vector2(x, y);
    }

    // gets the corner position of a rect relative to its anchored position. Good for when elements are in a layout group and you want the position relative to its top (or other aligned side)
    public static Vector2 RectCornerLocalPosition(RectTransform rxForm, Vector2 corner)
    {
        return Vector2.Scale(rxForm.anchoredPosition, corner - rxForm.pivot);
    }

    public static Vector2[] GetRectCorners(RectTransform rxForm)
    {
        Vector2[] corners = new Vector2[4];

        // it's all in the pivot
        Vector2 pivot = rxForm.pivot;
        
        // need to apply the parent scale to this child becasue scale trickles down. another reason keeping scale as 1,1,1 is ideal
        Canvas parentCanvas = rxForm.GetComponentInParent<Canvas>();
        Vector2 size = Vector2.Scale(rxForm.rect.size, parentCanvas.transform.localScale);

        // corners of the rect can be described same as the pivots. corner - pivot is the direction to the corner. scale that by the rect size and we have the extents of the rect 
        Vector2 rectPos = new Vector2(rxForm.position.x, rxForm.position.y);    // this will be relative to the overall world
        corners[0] = Vector2.Scale(Vector2.zero - pivot, size) + rectPos;
        corners[1] = Vector2.Scale(Vector2.up - pivot, size) + rectPos;
        corners[2] = Vector2.Scale(Vector2.one - pivot, size) + rectPos;
        corners[3] = Vector2.Scale(Vector2.right - pivot, size) + rectPos;

        return corners;
    }

    public static Vector2 MousePositionInRect(RectTransform rxForm, Vector2 mousePos)
    {
        // based on a mouse position on the screen, get where that hit on the rxForm\
        return Vector2.zero;
    }

    public static bool IsMouseInRect(Vector2 mousePos, RectTransform rxForm)
    {
        Vector2[] corners = GetRectCorners(rxForm);
        
        //Debug.Log($"{mousePos} ({corners[0].x}..{corners[3].x}) ({corners[0].y}..{corners[1].y})");
        return mousePos.x > corners[0].x && mousePos.x < corners[2].x && mousePos.y < corners[1].y && mousePos.y > corners[3].y;
    }
}
