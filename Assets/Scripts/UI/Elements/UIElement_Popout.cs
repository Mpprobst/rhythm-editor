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

    protected void Start()
    {
        Close();
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

    protected void CloseInternal()
    {
        if (onClose != null) onClose.Invoke();
        Close();
    }

    public PopoutOption GetOption<T>(string optionName)
    {
        return GetOption(optionName, typeof(T));
    }

    public T GetPopout<T>(string optionName)
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
