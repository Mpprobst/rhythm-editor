using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PopoutOption_Action : PopoutOption
{
    [SerializeField] private Button button;

    public UnityEvent onActivate = new UnityEvent();
    new protected Type valueType = typeof(bool);

    public override void SetInfo(ElementInputOptionData info)
    {
        base.SetInfo(info);
        button.onClick.AddListener(OnClick);
    }

    protected override float GetOptionHeight()
    {
        // unique case where we aren't calling base because the label and icon are contained within the button
        float h = 0;
        if (button)
            h += button.image.rectTransform.rect.height;
        if (description && !string.IsNullOrEmpty(description.text))
            h += description.rectTransform.rect.height;
        return h;
    }

    // may seem odd to add this layer between the Unity button and whatever this is listening to,
    // but it is a nice way of allowing other scripts to just look at this event instead of having to get the button and listen to that
    protected virtual void OnClick()
    {
        onActivate.Invoke();
    }

    public override void SetColors(UIStyleData style)
    {
        base.SetColors(style);
        button.image.color = style.backgroundColor_secondary;
    }

    public override void SetStyle(UIStyleData style)
    {
        base.SetStyle(style);
        button.image.sprite = style.buttonSprite;
    }
}
