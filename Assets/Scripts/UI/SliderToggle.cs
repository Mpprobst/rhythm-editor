using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class SliderToggle : MonoBehaviour, IUIStyle
{
    private bool isOn;
    public Image switchButton;
    public Image background;

    public Color onColor, offColor;

    public UnityEvent<bool> onValueChange;

    // set in the inspector by a Event Trigger
    public void OnClick()
    {
        SetValue(!isOn);

        if (onValueChange != null)
            onValueChange.Invoke(isOn);
    }

    public void SetValue(bool val)
    {
        isOn = val;
        // Adjusting anchors rather than position because this is more consistent
        // Sometimes this function is called before the UI sizes are updated, causing the handle to slide out of the bounds of the handler
        Vector2 minAnchor = new Vector2(isOn ? 1 : 0, 0);
        Vector2 maxAnchor = new Vector2(isOn ? 1 : 0, 1);
        Vector2 pivot = new Vector2(isOn ? 1 : 0, 0.5f);
        Vector2 size = new Vector2(switchButton.rectTransform.sizeDelta.x, -4);
        float x = isOn ? -2 : 2;

        // kill if we are pressing it in rapid succession
        switchButton.DOKill();
        background.DOKill();

        switchButton.rectTransform.DOAnchorMin(minAnchor, 0.2f);
        switchButton.rectTransform.DOAnchorMax(maxAnchor, 0.2f);
        switchButton.rectTransform.DOPivot(pivot, 0.2f);
        switchButton.rectTransform.DOSizeDelta(size, 0.2f);
        switchButton.rectTransform.DOAnchorPosX(x, 0.2f);
        background.DOColor(isOn ? onColor : offColor, 0.2f);
    }

    public bool GetValue()
    {
        return isOn;
    }

    public void SetColors(UIStyleData style)
    {
        offColor = style.textColor_secondary;
        onColor = style.backgroundColor_highlight;
        background.DOColor(isOn ? onColor : offColor, 0.2f);
        switchButton.DOColor(style.backgroundColor, 0.2f);
    }

    public void SetStyle(UIStyleData style)
    {
        // maybe set the sprites? not sure
        background.sprite = style.inputBackgroundSprite;
        background.type = style.isInputBackgroundTiled ? Image.Type.Tiled : Image.Type.Sliced;
        switchButton.sprite = style.handleSprite;

    }
}
