using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIElement_PopoutButton : UIElement_Button
{
    public UIElement_Popout popout; // when this is pressed, open this popout

    private bool firstOpen = true;

    public override void SetInfo(UIElementData data, LayoutAlignment alignment, ScreenSide screenSide)
    {
        base.SetInfo(data, alignment, screenSide);
        isToggle = true;
    }

    public override void SetActive(bool isOn)
    {
        base.SetActive(isOn);
        if (isActive)
        {
            if (firstOpen)
            {
                popout.SetPositionRelative(GetComponent<RectTransform>());
            }
            popout.onClose.RemoveListener(ClosePopout);
            popout.onClose.AddListener(ClosePopout);
            popout.Open();
        }
        else popout.Close();
    } 

    private void ClosePopout()
    {
        SetActiveNoNotify(false);
    }

    public override void SetActiveNoNotify(bool isOn)
    {
        base.SetActiveNoNotify(isOn);   // TODO: might actually need to send an event for 
    }

    public void SetPopoutPos()
    {
        popout.SetPositionRelative(GetComponent<RectTransform>());
    }

    public override void SetColors(UIStyleData style)
    {
        base.SetColors(style);
    }

    public override void SetStyle(UIStyleData style)
    {
        base.SetStyle(style);
    }
}
