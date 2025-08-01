using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILayout : MonoBehaviour
{
    public Transform primaryRibbon, secondaryRibbon, popoutContainer;
    public Transform window;    // where everything else will go

    public UIStyleData Style { get { return activeStyle; } }
    protected UIStyleData activeStyle;
    protected UIStyleData defaultStyle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
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
}
