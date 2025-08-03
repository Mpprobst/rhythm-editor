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
    }

    protected override void Awake()
    {
        base.Awake();
        button.onClick.AddListener(OnClick);
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
