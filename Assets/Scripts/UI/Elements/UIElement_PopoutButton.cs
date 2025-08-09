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
                firstOpen = false;
                SetPopoutPos();
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
        ScreenSide screenSide = Utils.ScreenSideOfElement(GetComponent<RectTransform>());
        RectTransform popoutRXForm = popout.GetComponent<RectTransform>();
        Vector2 anchorPivot = Vector2.one;// Utils.GetAnchorFromAlignment(screenSide, LayoutAlignment.CENTER);
        if (screenSide == ScreenSide.LEFT) anchorPivot.y = 0;
        else if (screenSide == ScreenSide.RIGHT) anchorPivot.y = 1;
        else if (screenSide == ScreenSide.BOTTOM) anchorPivot.x = 0;
        else anchorPivot.x = 1;

        Transform popoutParent = popoutRXForm.parent;
        popoutRXForm.SetParent(transform);
        popoutRXForm.anchorMin = anchorPivot;
        popoutRXForm.anchorMax = anchorPivot;
        popoutRXForm.pivot = anchorPivot;

        // pretty much puts the popout corner touching the corner of the button while being flush to its edge
        RectTransform buttonRect = GetComponent<RectTransform>();
        Vector2 popoutPos = Vector2.Scale(buttonRect.rect.size, -anchorPivot);
        popoutPos += Vector2.Scale(anchorPivot - Vector2.up, popoutRXForm.rect.size);

        popoutRXForm.anchoredPosition = popoutPos;

        popoutRXForm.SetParent(popoutParent);
        popoutRXForm.localScale = Vector3.one;
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
