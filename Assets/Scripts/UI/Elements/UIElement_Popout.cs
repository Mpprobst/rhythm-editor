using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Text = TMPro.TextMeshProUGUI;
using Unity.VisualScripting;

public class UIElement_Popout : MonoBehaviour, IUIStyle
{
    [SerializeField] protected Text titleLabel;
    [SerializeField] protected Image background, contentImage;

    public Transform content;   // where we put things

    [SerializeField] protected CanvasGroup canvasGroup;

    public void SetInfo(string title)
    {
        titleLabel.text = title;
        Close();
    }

    // should be openable from several sources
    // so we don't set what opens it from set info but rather whatever prompted this to exist
    public void Open()
    {
        canvasGroup.DOFade(1, 0.2f);
        canvasGroup.blocksRaycasts = true;
    }

    public void Close()
    {
        if (!Application.isPlaying)
            canvasGroup.alpha = 0;
        canvasGroup.DOFade(0, 0.2f);
        canvasGroup.blocksRaycasts = false;
    }

    public PopoutOption GetOption<T>(string optionName)
    {
        return GetOption(optionName, typeof(T));
    }

    public T GetPopout<T>(string optionName)
    {
        PopoutOption op = GetOption(optionName, typeof(T));

        if (op != null)
            return (T)op.ConvertTo(typeof(T));

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
            if (string.Compare(option.Name, optionName, true) > 0 && Utils.IsSubclass(option.GetType(), optionType))
                return option;

        return null;
    }

    public virtual void SetColors(UIStyleData style)
    {
        titleLabel.color = style.textColor_primary;
        background.color = style.backgroundColor;
        contentImage.color = style.backgroundColor_secondary;
    }

    public virtual void SetStyle(UIStyleData style)
    {
        titleLabel.font = style.font;
        background.sprite = style.backgroundSprite;
    }
}
