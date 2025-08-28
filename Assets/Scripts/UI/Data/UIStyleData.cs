using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "UIStyleData", menuName = "ScriptableObjects/UI/UIStyleData", order = 1)]
public class UIStyleData : ScriptableObject
{
    public Color backgroundColor;
    public Color backgroundColor_secondary;
    public Color backgroundColor_tertiary;
    public Color backgroundColor_highlight;

    public Color iconColor;
    public Color iconColor_highlight;
    public Color textColor_primary;
    public Color textColor_secondary;
    // no text highlight because it will use the same as icon

    public Sprite backgroundSprite;
    public Sprite inputBackgroundSprite;
    public Sprite buttonSprite;
    public Sprite handleSprite;
    public TMPro.TMP_FontAsset font;

}
