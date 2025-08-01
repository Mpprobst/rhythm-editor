using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Text = TMPro.TextMeshProUGUI;

public class UIElement_Label : UIElement
{
    [SerializeField] protected Text label;
    [SerializeField] protected Image icon;
    [SerializeField] protected LayoutElement element;
    [SerializeField] protected AspectRatioFitter contentRatioFitter;

    public override void SetInfo(UIElementData data, LayoutAlignment alignment, ScreenSide screenSide)
    {
        elementName = data.elementName;
        // label and icon are in the same layout group.
        label.text = data.elementName;
        label.alignment = Utils.GetTextAlignmentOption(screenSide, alignment);
        icon.sprite = data.icon;
        icon.gameObject.SetActive(data.icon != null);

        if (element)
        {
            RectTransform parentRect = transform.parent.GetComponent<RectTransform>();
            bool widthControlled = screenSide == ScreenSide.LEFT || screenSide == ScreenSide.RIGHT;
            if (widthControlled)
            {
                element.preferredHeight = parentRect.rect.width;
                element.flexibleHeight = 0; // keeps it locked to this max height
                element.preferredWidth = 0;
                element.flexibleWidth = 0;
            }
            else
            {
                element.preferredWidth = parentRect.rect.height;
                element.flexibleWidth = 1;  // allows it to expand
                element.preferredHeight = 0;
                element.flexibleHeight = 0; // keeps it locked to this max height
            }
        }

        if (contentRatioFitter)
        {
            if (data.icon)
            {
                contentRatioFitter.aspectMode = screenSide == ScreenSide.LEFT || screenSide == ScreenSide.RIGHT ? AspectRatioFitter.AspectMode.WidthControlsHeight : AspectRatioFitter.AspectMode.HeightControlsWidth;
                contentRatioFitter.aspectRatio = data.icon.textureRect.width / data.icon.textureRect.height;
            }
        }
    }

    public override void SetColors(UIStyleData style)
    {
        label.color = style.textColor_primary;
        icon.color = style.iconColor;
    }

    public override void SetStyle(UIStyleData style)
    {
        label.font = style.font;
    }
}
