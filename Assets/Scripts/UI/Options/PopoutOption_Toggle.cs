using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PopoutOption_Toggle : PopoutOption
{
    // TODO a slider class
    [SerializeField] private SliderToggle toggle;

    public UnityEvent<bool> onValueChange = new UnityEvent<bool>();

    public override void SetInfo(ElementInputOptionData info)
    {
        base.SetInfo(info);
        valueType = typeof(bool);
    }

    protected override void Awake()
    {
        base.Awake();
        value = false;
        toggle.onValueChange.AddListener(SetValue);
    }

    public bool GetValue()
    {
        return (bool)value;
    }

    public void SetValue(bool val)
    {
        base.SetValue(val);
        onValueChange.Invoke(val);
    }

    public void SetValueNoNotify(bool value)
    {
        base.SetValue(value);
        toggle.SetValue(value);
    }

    public override void SetColors(UIStyleData style)
    {
        base.SetColors(style);
        // toggle has its own IUIStyle
    }

    public override void SetStyle(UIStyleData style)
    {
        base.SetStyle(style);
    }
}
