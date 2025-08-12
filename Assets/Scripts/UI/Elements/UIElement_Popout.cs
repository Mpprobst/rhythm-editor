using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using Text = TMPro.TextMeshProUGUI;
using System.Reflection;
using Unity.VisualScripting;    // for the object.ConvertTo(typeof(T)) because we cant do System.Convert(obj, typeof(T)) anymore

// TODO: drag
public class UIElement_Popout : MonoBehaviour, IUIStyle
{
    [SerializeField] protected Text titleLabel;
    [SerializeField] protected Image background, contentImage;
    [SerializeField] protected Button closeButton;

    public Transform content;   // where we put things

    [SerializeField] protected CanvasGroup canvasGroup;

    public UnityEvent onClose;  // if the popup closes itself and something needs to know about it. notably the button that opened it so it can return to a normal color state

    public void SetInfo(string title)
    {
        titleLabel.text = title;
        //Close();
    }

    protected virtual void Start()
    {
        Close();
    }

    // aligns this popout relative to a UI element and its screen position
    public void SetPositionRelative(RectTransform relativeRect)
    {
        ScreenSide screenSide = Utils.ScreenSideOfElement(relativeRect);
        RectTransform popoutRXForm = GetComponent<RectTransform>();
        Vector2 anchorPivot = Utils.GetScreenCorner(relativeRect);
        popoutRXForm.anchorMin = anchorPivot;
        popoutRXForm.anchorMax = anchorPivot;
        popoutRXForm.pivot = anchorPivot;

        // alterations make it the opposite side of the relativeRect we are setting to
        //Vector2 anchorPivot = Vector2.zero;// Utils.GetAnchorFromAlignment(screenSide, LayoutAlignment.CENTER);
        if (screenSide == ScreenSide.LEFT) anchorPivot.x = 1; 
        else if (screenSide == ScreenSide.RIGHT) anchorPivot.x = 0; 
        else if (screenSide == ScreenSide.BOTTOM) anchorPivot.y = 1;
        else anchorPivot.y = 0;
        
        // pretty much puts the popout corner touching the corner of the button while being flush to its edge
        // use the opposite anchor thing to add the rect size
        Vector2 popoutPos = Vector2.Scale(relativeRect.rect.size , anchorPivot-relativeRect.pivot) + new Vector2(relativeRect.position.x, relativeRect.position.y);
        // using the screen corner we don't need to add any more position based on the size of this popout
        Debug.Log("popoutPos " + popoutPos);
        popoutRXForm.position = popoutPos;

        GetComponent<ScreenConstraint>()?.CheckPosition();
    }

    // should be openable from several sources
    // so we don't set what opens it from set info but rather whatever prompted this to exist
    public void Open()
    {
        canvasGroup.DOFade(1, 0.2f);
        canvasGroup.blocksRaycasts = true;

        closeButton.onClick.RemoveListener(CloseInternal);
        closeButton.onClick.AddListener(CloseInternal);
    }

    public void Close()
    {
        if (!Application.isPlaying)
            canvasGroup.alpha = 0;
        canvasGroup.DOFade(0, 0.2f);
        canvasGroup.blocksRaycasts = false;
    }

    // TODO: a set position function relative to a general rect. not controlled by the popout button

    protected void CloseInternal()
    {
        if (onClose != null) onClose.Invoke();
        Close();
    }

    public T GetOption<T>(string optionName)
    {
        PopoutOption op = GetOption(optionName, typeof(T));

        if (op != null)
            return (T)op.ConvertTo(typeof(T));//return (T)Convert.ChangeType(op, typeof(T));

        Debug.LogError($"Could not find option {optionName} of type {typeof(T).Name} in popout: {name}");
        return default(T);
    }

    public PopoutOption GetOption(string optionName, Type optionType)
    {
        if (optionType == null || !Utils.IsSubclass(optionType, typeof(PopoutOption)))
        {
            Debug.LogError($"Tried to get option {optionName} with a type that is either not a subclass of PopoutOption or is null");
            return null;
        }

        var options =  content.GetComponentsInChildren<PopoutOption>();
        foreach (var option in options)
            if (option.optionName.Equals(optionName, StringComparison.InvariantCultureIgnoreCase) && Utils.IsSubclass(option.GetType(), optionType))
                return option;

        return null;
    }

    public virtual void SetColors(UIStyleData style)
    {
        titleLabel.color = style.textColor_primary;
        background.color = style.backgroundColor;
        contentImage.color = style.backgroundColor_secondary;
        closeButton.image.color = style.backgroundColor_secondary;
        closeButton.GetComponentInChildren<Text>().color = style.textColor_secondary;
    }

    public virtual void SetStyle(UIStyleData style)
    {
        titleLabel.font = style.font;
        background.sprite = style.backgroundSprite;
        closeButton.image.sprite = style.buttonSprite;
        closeButton.GetComponentInChildren<Text>().font = style.font;
    }
}
