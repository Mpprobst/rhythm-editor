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

    protected Color baseColor, highlightColor;

    public override void SetInfo(UIElementData data, LayoutAlignment alignment, ScreenSide screenSide)
    {
        base.SetInfo(data, alignment, screenSide);
        isToggle = data.isToggle;   
        button.onClick.AddListener(OnClick);
    }

    public virtual void OnClick()
    {
        if (isToggle) isActive = !isActive;
        SetActive(isActive);
        onActivate.Invoke(isActive);
    }

    public virtual void SetActive(bool isOn)
    {
        isActive = isOn;
        button.image.color = isActive ? highlightColor : baseColor;
        if (onSelect != null)
            onSelect.Invoke(id);
    }

    public override void SetColors(UIStyleData style)
    {
        base.SetColors(style);
        button.image.color = style.backgroundColor;
        baseColor = style.backgroundColor;
        highlightColor = style.backgroundColor_highlight;
    }


    public override void SetStyle(UIStyleData style)
    {
        base.SetStyle(style);
        button.image.sprite = style.buttonSprite;
    }
}
