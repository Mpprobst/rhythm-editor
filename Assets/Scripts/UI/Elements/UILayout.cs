using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILayout : MonoBehaviour
{
    public Transform primaryRibbon, secondaryRibbon, popoutContainer;
    public Transform window;    // where everything else will go

    public UIStyleData Style { get { return activeStyle; } }
    [SerializeField] protected UIStyleData activeStyle;
    [SerializeField] protected UIStyleData defaultStyle;

    protected virtual void Awake()
    {

    }

    public void SetStyles(UIStyleData style)
    {
        defaultStyle = style;
        activeStyle = defaultStyle;
    }

    // TODO: dark mode

    // TODO: functions to help find buttons in the ui

    public void SetElementColors()
    {
        if (activeStyle == null)
        {
            Debug.LogWarning("Cannot set style. No active style");
            return;
        }

        if (primaryRibbon)
            primaryRibbon.GetComponent<Image>().color = activeStyle.backgroundColor;
        if (primaryRibbon)
            secondaryRibbon.GetComponent<Image>().color = activeStyle.backgroundColor;
        var elements = GetComponentsInChildren<IUIStyle>();
        foreach (var element in elements)
            element.SetColors(activeStyle);
    }

    public void SetElementStyle()
    {
        if (activeStyle == null)
        {
            Debug.LogWarning("Cannot set style. No active style");
            return;
        }

        var elements = GetComponentsInChildren<IUIStyle>();
        foreach (var element in elements)
            element.SetStyle(activeStyle);
    }

    public T GetElement<T>(string name, Transform parent = null ) where T : UIElement
    {
        return (T)GetElement(name, typeof(T), parent);
    }

    public UIElement GetElement(string elementName, Type type, Transform parent = null)
    {
        if (parent == null)
        {
            UIElement element = null;
            if (primaryRibbon) 
                element = GetElement(elementName, type, primaryRibbon);
            if (element == null && secondaryRibbon)
                element = GetElement(elementName, type, secondaryRibbon);
            return element;
        }
        // somehow parent.getchild is not getting the actual children
        Debug.Log($"{parent.name} has {parent.childCount} children");
        for (int i = 0; i < parent.childCount; i++)
        {
            UIElement element = parent.GetChild(i).GetComponent<UIElement>();
            Debug.Log($"child {parent.GetChild(i).name} has element? {element} ");
            if (element && element.GetType() == type && element.ElementName.Contains(elementName))
                return element;
        }
        return null;
    }
}
