using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// this is intended to be reused throughout the scene
public class ColorPopout : UIElement_Popout
{
    public static ColorPopout Instance { get { return _instance; } }
    private static ColorPopout _instance;

    PopoutOption_Number redOption, greenOption, blueOption;

    public UnityEvent<Color> onColorChange = new UnityEvent<Color>();

    protected override void Start()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        base.Start();
        redOption = GetOption<PopoutOption_Number>("Red");
        redOption.onNumberChanged.AddListener(SetRed);

        greenOption = GetOption<PopoutOption_Number>("Green");
        greenOption.onNumberChanged.AddListener(SetGreen);

        blueOption = GetOption<PopoutOption_Number>("Blue");
        blueOption.onNumberChanged.AddListener(SetBlue);
    }

    private void SetRed(float r)
    {
        SetColor(r, greenOption.GetValue(), blueOption.GetValue());
    }
    private void SetGreen(float g)
    {
        SetColor(redOption.GetValue(), g, blueOption.GetValue());
    }

    private void SetBlue(float b)
    {
        SetColor(redOption.GetValue(), greenOption.GetValue(), b);
    }

    public void SetColorNoNotify(Color color)
    {
        redOption.SetValue(color.r);
        greenOption.SetValue(color.g);
        blueOption.SetValue(color.b);
    }

    private void SetColor(float r, float g, float b)
    {   
        Color c = new Color(r, g, b);
        onColorChange.Invoke(c);
        Debug.Log("set color " + c);
    }

    public Color GetColor()
    {
        Color c = new Color(
            redOption.GetValue(),
            greenOption.GetValue(),
            blueOption.GetValue()
        );

        return c;
    }


}
