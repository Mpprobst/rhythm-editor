using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Text = TMPro.TextMeshProUGUI;

public class UIElement_Label : UIElement
{
    [SerializeField] protected Text label;
    [SerializeField] protected Image icon;

    public override void SetInfo(UIElementData data)
    {
        label.text = data.elementName;
        icon.sprite = data.icon;
    }

    public override void SetColors(UIStyleData style)
    {
        throw new System.NotImplementedException();
    }


    public override void SetStyle(UIStyleData style)
    {
        throw new System.NotImplementedException();
    }
}
