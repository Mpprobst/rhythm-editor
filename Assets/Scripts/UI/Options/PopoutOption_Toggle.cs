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
        toggle.onValueChange.AddListener(SetValue);
        valueType = typeof(bool);
    }

    protected override float GetOptionHeight()
    {
        float h = base.GetOptionHeight();
        // don't need toggle height because it is in line with the icon/title
        //if (toggled)
        //    h += toggle.GetComponent<RectTransform>().rect.height;
        return h;
    }

    public bool GetValue()
    {
        return GetValue<bool>();
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
