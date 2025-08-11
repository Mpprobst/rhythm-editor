using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Text = TMPro.TextMeshProUGUI;
using DG.Tweening;
using Unity.VisualScripting;


public abstract class PopoutOption : MonoBehaviour, IUIStyle
{
    [SerializeField] protected Text label, description;
    [SerializeField] protected Image icon;

    protected object value;
    protected Type valueType;
    public string optionName;

    public virtual void SetInfo(ElementInputOptionData info)
    {
        optionName = info.optionName;
        if (label)
        {
            label.text = optionName;
            label.gameObject.SetActive(info.icon == null);
        }
        if (description)
        {
            description.text = info.description;
            description.gameObject.SetActive(!string.IsNullOrEmpty(description.text));
        }
        if (icon)
        {
            icon.sprite = info.icon;
            icon.gameObject.SetActive(icon.sprite != null);
        }
    }

    public virtual void SetName(string val)
    {
        label.text = val;
    }

    protected virtual void Awake()
    {
        // must setup on value change events in awake (or in inspector but this feels better to me
    }

    // WARNING: elements can have height set to 0 due to previously failed generations of UI. Shouldn't be happening anymore but it is a known issue
    protected virtual float GetOptionHeight()
    {
        float h = 0;
        if (label.transform.parent.GetComponent<LayoutGroup>())    // several option labels will be contained by another rect. if so, the label's height is like 0 because it relies on the layout group for its height.
            h += label.transform.parent.GetComponent<RectTransform>().rect.height;
        else
            h += label.rectTransform.rect.height;
        if (description && !string.IsNullOrEmpty(description.text))
            h += description.rectTransform.rect.height;
        // icon usually within the same component as the label and we will always have a label
        return h;
    }

    // more efficient than a ton of content size fitters
    // with too many, the editor really starts to struggle
    public void SetOptionHeight()
    {
        float h = GetOptionHeight();
        if (h < 20)
        {
            Debug.LogWarning($"Very small height detected for {name} {h}. Please make sure no child elements have a proper height. If problem persists, delete the prefab and all scene references and regenerate");
        }

        RectTransform rxForm = GetComponent<RectTransform>();
        rxForm.sizeDelta = new Vector2(rxForm.rect.width, h);
        if (rxForm.GetComponent<LayoutGroup>() != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(rxForm);    // applies layout group params to content
    }

    protected T GetValue<T>()
    {
        if (typeof(T) == valueType)
        {
            return (T)Convert.ChangeType(value, valueType);
        }
        return default(T);
    }

    protected void SetValue(object val)
    {
        //value.ConvertTo(valueType);
        value = val;
        Debug.Log($"setting {optionName} value to " + value);
    }
    
    public virtual void SetColors(UIStyleData style)
    {
        if (label)
            label.color = style.textColor_primary;
        if (icon)
            icon.color = style.iconColor;
    }
    public virtual void SetStyle(UIStyleData style)
    {
        if (label)
            label.font = style.font;
    }
}
