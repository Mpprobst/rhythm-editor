using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIElement_Button : UIElement_Label
{
    [SerializeField] protected Button button;

    public UnityEvent<bool> onActivate = new UnityEvent<bool>();
    public UnityEvent<int> onSelect = new UnityEvent<int>();

    [SerializeField] protected bool isToggle;
    protected bool isActive;
    public int id;

    [Header("Colors")]
    [SerializeField] protected Color baseColorBackground, highlightColorBackground;
    [SerializeField] protected Color baseColorLabel, highlightColorLabel;

    public override void SetInfo(UIElementData data, LayoutAlignment alignment, ScreenSide screenSide)
    {
        base.SetInfo(data, alignment, screenSide);
        isToggle = data.isToggle;   
    }

    public virtual void OnClick()
    {
        if (isToggle) isActive = !isActive;
        SetActive(isActive);
        onActivate.Invoke(isActive);
    }

    public virtual void SetActive(bool isOn)
    {
        if (isToggle) isActive = isOn; // looks weird, but only toggles really care about button status
        SetActiveNoNotify(isOn);
        if (onSelect != null)
            onSelect.Invoke(id);
    }

    public virtual void SetActiveNoNotify(bool isOn)
    {
        isActive = isOn;
        button.image.color = isActive ? highlightColorBackground : baseColorBackground;
        label.color = isActive ? highlightColorLabel : baseColorLabel;
        icon.color = isActive ? highlightColorLabel : baseColorLabel;
    }

    public override void SetColors(UIStyleData style)
    {
        base.SetColors(style);
        button.image.color = style.backgroundColor;
        baseColorBackground = style.backgroundColor;
        highlightColorBackground = style.backgroundColor_highlight;
        baseColorLabel = style.textColor_primary;
        highlightColorLabel = style.iconColor_highlight;
    }


    public override void SetStyle(UIStyleData style)
    {
        base.SetStyle(style);
        button.image.sprite = style.buttonSprite;
    }
}
