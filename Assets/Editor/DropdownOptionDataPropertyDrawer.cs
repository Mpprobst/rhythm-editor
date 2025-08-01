using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DropdownOptionData))]
public class DropdownOptionDataPropertyDrawer : PropertyDrawer
{
    private SerializedProperty optionName_prop, optionIcon_prop;

    private void Init(SerializedProperty property)
    {
        optionName_prop = property.FindPropertyRelative("optionName");
        optionIcon_prop = property.FindPropertyRelative("optionIcon");
    }

    // if you don't use the serialized properties, your changes in the inspector window will not apply to your object!
    // ex: you instead get the class instance and set UIElementData.elementName directly with your EditorGUIs will not work.
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        Init(property);

        EditorGUI.BeginProperty(rect, label, property);

        EditorGUI.indentLevel = 0;
      
        //Rect dropdownRect = EditorGUI.PrefixLabel(rect, EditorGUIUtility.GetControlID(FocusType.Passive), new GUIContent("Option"));
        float iconWidth = 80f;
        Rect iconRect = new Rect(rect.x, rect.y, iconWidth, EditorGUIUtility.singleLineHeight);
        Rect nameRect = new Rect(rect.x + iconWidth, rect.y, rect.width - iconWidth, EditorGUIUtility.singleLineHeight);

        EditorGUI.ObjectField(iconRect, optionIcon_prop, GUIContent.none);
        EditorGUI.PropertyField(nameRect, optionName_prop, GUIContent.none);
    } 

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int numProperties = 1;
        return EditorGUIUtility.singleLineHeight * numProperties;
    }
}
