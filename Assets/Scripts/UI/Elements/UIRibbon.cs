using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRibbon : MonoBehaviour, IUIStyle
{
    [SerializeField] private Image background;
    public bool isPrimary;
    public ScreenSide screenSide;

    public void SetColors(UIStyleData style)
    {
        if (background)
            background.color = isPrimary ? style.backgroundColor_secondary : style.backgroundColor_tertiary;
    }

    public void SetStyle(UIStyleData style)
    {
        if (background)
        {
            background.sprite = style.buttonSprite;
            background.type = style.isButtonTiled ? Image.Type.Tiled : Image.Type.Sliced;
        }
    }
}
