using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIElement_PopoutButton : UIElement_Button
{
    public UIElement_Popout popout; // when this is pressed, open this popout

    public override void SetInfo(UIElementData data, LayoutAlignment alignment, ScreenSide screenSide)
    {
        base.SetInfo(data, alignment, screenSide);
        isToggle = true;
    }

    public override void SetActive(bool isOn)
    {
        base.SetActive(isOn);
        if (isActive) popout.Open();
        else          popout.Close();
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
