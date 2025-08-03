using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Text = TMPro.TextMeshProUGUI;
using InputField = TMPro.TMP_InputField;
using System;

public class PopoutOption_Text : PopoutOption
{
    [SerializeField] protected InputField inputField;
    public UnityEvent<string> onValueEnter = new UnityEvent<string>();
    new protected Type valueType = typeof(string);

    public override void SetInfo(ElementInputOptionData info)
    {
        base.SetInfo(info);
        valueType = typeof(string);
        inputField.placeholder.GetComponent<Text>().text = info.placeholder;
    }

    protected override void Awake()
    {
        base.Awake();
        inputField.onValueChanged.AddListener(SetValue);
    }

    protected override float GetOptionHeight()
    {
        float h = base.GetOptionHeight();
        if (inputField)
            h += inputField.image.rectTransform.rect.height;
        return h;
    }

    // Action doesn't really have a value, but the other inhereting classes will so 
    public string GetValue()
    {
        return GetValue<string>();
    }

    public virtual void SetValue(string val)
    {
        base.SetValue(val);
        onValueEnter.Invoke(val);
    }

    public virtual void SetValueNoNotify(string value)
    {
        base.SetValue(value);
        if (Application.isPlaying)  // causing an issue when generating
            inputField.SetTextWithoutNotify(value);
    }

    public override void SetColors(UIStyleData style)
    {
        base.SetColors(style);
        inputField.textComponent.color = style.textColor_primary;
        inputField.placeholder.color = style.textColor_secondary;
        inputField.image.color = style.backgroundColor_secondary;
    }

    public override void SetStyle(UIStyleData style)
    {
        base.SetStyle(style);
        inputField.textComponent.font = style.font;
        inputField.placeholder.GetComponent<Text>().font = style.font;
        inputField.image.sprite = style.buttonSprite;
    }
}
