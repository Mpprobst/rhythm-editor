using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExamplePopout : UIElement_Popout
{

    private PopoutOption_Action actionOption;
    private PopoutOption_Toggle toggleOption;
    private PopoutOption_Text textOption;
    private PopoutOption_Number numberOption;
    private PopoutOption_Dropdown dropdownOption;

    protected void Awake()
    {
        actionOption = GetPopout<PopoutOption_Action>("Action");
        actionOption.onActivate.AddListener(OnAction);
        
        toggleOption = GetPopout<PopoutOption_Toggle>("Toggle");
        toggleOption.onValueChange.AddListener(OnToggle);

        textOption = GetPopout<PopoutOption_Text>("Text");
        textOption.onValueEnter.AddListener(OnText);

        numberOption = GetPopout<PopoutOption_Number>("Integer");
        numberOption.onNumberChanged.AddListener(OnInteger);

        dropdownOption = GetPopout<PopoutOption_Dropdown>("Dropdown");
        dropdownOption.onValueChanged.AddListener(OnDropdown);
    }

    private void OnAction()
    {
        Debug.Log("Action!");
    }

    private void OnToggle(bool val)
    {
        Debug.Log($"Toggle {val}");
    }

    private void OnText(string val)
    {
        Debug.Log($"Text: {val}");
    }

    private void OnInteger(float val)
    {
        Debug.Log($"Num: {val}");
    }

    private void OnDropdown(int val)
    {
        Debug.Log($"Dropdown: {val} ({dropdownOption.GetOption(val)})");
    }

}


