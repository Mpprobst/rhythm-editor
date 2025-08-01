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
    public string Name { get { return optionName; } }
    private string optionName;


    // sums up the height of active RectTransforms as we set information. Needed really only for generated elements on setup
    // cached because we want to use function overrides.

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

    // WARNING: elements can have height set to 0 due to previously failed generations of UI. Shouldn't be happening anymore but it is a known issue
    protected virtual float GetOptionHeight()
    {
        float h = 0;
        if (label)
            h += label.rectTransform.rect.height;
        if (description && !string.IsNullOrEmpty( description.text))
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
            Debug.LogWarning($"Very small height detected for {name}. Please make sure no child elements have a proper height. If problem persists, delete the prefab and all scene references and regenerate");
        }

        Debug.Log($"{name} height - {h}");
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
