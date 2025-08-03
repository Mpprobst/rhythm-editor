using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Marks what fields we should skip in reflection in our editor script
public class SkipFieldAttribute : System.Attribute {}

public enum UIElementType { LABEL, BUTTON, POPOUT }
public class ElementTypeAttribute : System.Attribute
{
    public UIElementType ElementType;
    public ElementTypeAttribute(UIElementType type)
    {
        ElementType = type;
    }
}

public enum StackingDirection { NONE, HORIZONTAL, VERTICAL }

[CreateAssetMenu(fileName = "UIElementData", menuName = "ScriptableObjects/UI/UIElementData", order = 2)]
public class UIElementData : ScriptableObject
{
    [SkipField] public UIElementType elementType;
    [HideInInspector] public GameObject elementPrefab;

    public string elementName;
    public string description;
    public Sprite icon;
     [Tooltip("How the name and icon will be displayed in relation to one another. If NONE, only either icon or name will be displayed")] 
    public StackingDirection stacking;

    // Button and Popout only
    [ElementType(UIElementType.BUTTON)] public KeyCode[] shortcut;
    [ElementType(UIElementType.BUTTON)] public bool isToggle;

    // Popout only
    [ElementType(UIElementType.POPOUT)] public ElementInputOptionData[] popoutOptions = new ElementInputOptionData[0];
    [ElementType(UIElementType.POPOUT)] public GameObject popoutPrefab;

}


// Option Data - Will be displayed in popouts
public enum OptionType { ACTION, TOGGLE, TEXT, NUMBER, DROPDOWN, FILE }
public class OptionTypeAttribute : System.Attribute 
{
    public OptionType Type;
    public OptionTypeAttribute(OptionType type)
    {
        Type = type;
    }
}


// Data describing something that will provide input to an element data
[System.Serializable]
public class ElementInputOptionData
{
    public static Dictionary<OptionType, OptionType[]> compatibleTypes = new Dictionary<OptionType, OptionType[]>()
    {
        { OptionType.DROPDOWN, new OptionType[] { OptionType.ACTION, OptionType.DROPDOWN} },
        { OptionType.FILE, new OptionType[] { OptionType.ACTION, OptionType.FILE} },
    };

    public static bool OptionTypeComptaible(OptionType target, OptionType goal)
    {
        if (compatibleTypes.ContainsKey(target))
            return compatibleTypes[target].Contains(goal);
        return target >= goal;
    }

    // A button, text, number, or dropdown item
    [SkipField] public OptionType optionType;
    public string optionName;
    public string description;
    public Sprite icon;
    [SkipField] public GameObject optionPrefab;

    // text only
    [OptionType(OptionType.TEXT)] public string placeholder;

    // number only
    [OptionType(OptionType.NUMBER)] public float min;
    [OptionType(OptionType.NUMBER)] public float max;
    [OptionType(OptionType.NUMBER)] public float defaultValue;
    [OptionType(OptionType.NUMBER)] public float scale;
    [OptionType(OptionType.NUMBER)] public bool useSlider;
    [OptionType(OptionType.NUMBER)] public bool roundToInt;

    // dropdown only
    [OptionType(OptionType.DROPDOWN)] public DropdownOptionData[] options = new DropdownOptionData[0];

    // file only
    [OptionType(OptionType.FILE)] public FileType fileType;
}

[Serializable]
public class DropdownOptionData
{
    public string optionName;
    public Sprite optionIcon;
}

public enum FileType { MP3, IMAGE, TRACK }