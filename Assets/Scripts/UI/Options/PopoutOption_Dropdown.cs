using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Dropdown = TMPro.TMP_Dropdown;
using Text = TMPro.TextMeshProUGUI;
using Unity.VisualScripting;

public class PopoutOption_Dropdown : PopoutOption
{
    [SerializeField] private Dropdown dropdown;

    public UnityEvent<int> onValueChanged = new UnityEvent<int>();
    public UnityEvent<string> onStringValueChanged = new UnityEvent<string>();

    public override void SetInfo(ElementInputOptionData info)
    {
        base.SetInfo(info);
        valueType = typeof(int);
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        for (int i = 0; i < info.options.Length; i++)
        {
            options.Add(new Dropdown.OptionData(info.options[i].optionName, info.options[i].optionIcon));
        }
        dropdown.options = options;
        value = 0;
    }
    
    public int GetValue()
    {
        return (int)value;
    }

    protected override void Awake()
    {
        base.Awake();
        value = 0;
        dropdown.onValueChanged.AddListener(SetValue);
    }

    protected override float GetOptionHeight()
    {
        float h = base.GetOptionHeight();
        h += dropdown.GetComponent<RectTransform>().rect.height;
        return h;
    }

    public void SetValue(int val)
    {
        base.SetValue(val);
        dropdown.captionImage.GetComponent<NullSpriteChecker>()?.CheckSprite();
        onValueChanged.Invoke(val);
        onStringValueChanged.Invoke(dropdown.options[val].text);
    }

    public void SetValueNoNotify(string val)
    {
        for (int i = 0; i < dropdown.options.Count; i++)
        {
            if (string.Compare(dropdown.options[i].text, val, true) > 0)
            {
                SetValueNoNotify(i);
                break;
            }

        }
    }

    public void SetValueNoNotify(int val)
    {
        base.SetValue(val);
        dropdown.SetValueWithoutNotify(val);
    }

    public string GetOption(int val)
    {
        if (val < 0 || val >= dropdown.options.Count) return "";
        return dropdown.options[val].text;
    }

    public Sprite GetOptionImage(int val)
    {
        if (val < 0 || val >= dropdown.options.Count) return null;
        return dropdown.options[val].image;
    }

    public override void SetColors(UIStyleData style)
    {
        base.SetColors(style);
        dropdown.image.color = style.backgroundColor_secondary;
        dropdown.captionText.color = style.textColor_primary;
        dropdown.template.GetComponentInChildren<Text>().color = style.textColor_secondary;
    }

    public override void SetStyle(UIStyleData style)
    {
        base.SetStyle(style);
        dropdown.image.sprite = style.buttonSprite;
        dropdown.captionText.font = style.font;
        dropdown.template.GetComponentInChildren<Text>().font = style.font;
    }

}
