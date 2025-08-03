using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PopoutOption_Number : PopoutOption_Text
{
    [SerializeField] private Slider slider;
    //new protected Type valueType = typeof(float);

    public UnityEvent<float> onNumberChanged = new UnityEvent<float>();

    [SerializeField] float scale;

    public override void SetInfo(ElementInputOptionData info)
    {
        base.SetInfo(info);
        valueType = typeof(float);
        if (info.useSlider)
        {
            slider.minValue = info.min;
            slider.maxValue = info.max;
            slider.SetValueWithoutNotify(info.defaultValue);
            slider.wholeNumbers = info.roundToInt;
        }
        else
        {
            slider.gameObject.SetActive(false);
        }
        scale = info.scale;
    }

    protected override void Awake()
    {
        base.Awake();
        slider.onValueChanged.AddListener(SetValue);
    }

    protected override float GetOptionHeight()
    {
        float h = base.GetOptionHeight();
        // if the text input is there, we don't need to add any more height
        // however if we are only using the slider, we use the height of the rect that contains both the slider and the possible text input
        if (inputField == null)
            h += slider.transform.parent.GetComponent<RectTransform>().rect.height;
        return h;
    }

    new public float GetValue()
    {
        return GetValue<float>() * scale;
    }

    public override void SetValue(string value)
    {
        SetValue(float.Parse(value));
    }

    public override void SetValueNoNotify(string value)
    {
        SetValueNoNotify(float.Parse(value));
    }

    public void SetValue(float val)
    {
        val = val / scale;
        SetValueNoNotify(val); 
        onNumberChanged.Invoke(val);
    }

    public void SetValueNoNotify(float val)
    {
        if (slider)
            slider.SetValueWithoutNotify(val);
        base.SetValueNoNotify(val.ToString());
    }

    public override void SetColors(UIStyleData style)
    {
        base.SetColors(style);
        slider.image.color = style.backgroundColor_secondary;
        slider.handleRect.GetComponent<Image>().color = style.backgroundColor;
    }

    public override void SetStyle(UIStyleData style)
    {
        base.SetStyle(style);
        slider.image.sprite = style.backgroundSprite;
        slider.handleRect.GetComponent<Image>().sprite = style.buttonSprite;
    }
}
