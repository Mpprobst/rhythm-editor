using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ElementInputOptionData))]
public class ElementOptionPropertyDrawer : PropertyDrawer
{
    private SerializedProperty optionType_prop, optionPrefab_prop;
    private SerializedProperty[] fieldProperties;

    private FieldInfo[] fields;

    private void Init(SerializedProperty property)
    {
        optionType_prop = property.FindPropertyRelative("optionType");
        optionPrefab_prop = property.FindPropertyRelative("optionPrefab");

        fields = typeof(ElementInputOptionData).GetFields(BindingFlags.Public | BindingFlags.Instance);
        fieldProperties = new SerializedProperty[fields.Length];

        for (int i = 0; i < fields.Length; i++)
        {
            if (fields[i].GetCustomAttribute<HideInInspector>() != null || fields[i].GetCustomAttribute<SkipFieldAttribute>() != null) continue;
            fieldProperties[i] = property.FindPropertyRelative(fields[i].Name);
        }
    }

    // if you don't use the serialized properties, your changes in the inspector window will not apply to your object!
    // ex: you instead get the class instance and set UIElementData.elementName directly with your EditorGUIs will not work.
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        Init(property);

        int indent = EditorGUI.indentLevel;
        EditorGUI.BeginProperty(rect, label, property);

        EditorGUI.indentLevel = 0;

        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), optionType_prop);
        rect.y += EditorGUIUtility.singleLineHeight;    // must manually inform the editor when to add spacing between elemements unlike in custom Editors

        OptionType optionType = (OptionType)optionType_prop.enumValueIndex;

        // reflection here is way more scalable, especially if we add new fields to our class later
        // otherwise we are finding 
        for (int i = 0; i < fieldProperties.Length; i++)
        {
            if (fieldProperties[i] == null) continue;
            if (fields[i].GetCustomAttribute<HideInInspector>() != null || fields[i].GetCustomAttribute<SkipFieldAttribute>() != null) continue;

            var typeAttrib = fields[i].GetCustomAttribute<OptionTypeAttribute>();
            if (typeAttrib == null || ElementInputOptionData.OptionTypeComptaible(optionType, typeAttrib.Type))
            {
                // draw the property
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), fieldProperties[i]);
                rect.y += EditorGUIUtility.singleLineHeight;
                if (fieldProperties[i].isArray)
                {
                    // TODO: if this is a dropdown, we should put the elements next to one another
                    // or create a custom property drawer for dropdown elements?
                    rect.y += EditorGUIUtility.singleLineHeight * fieldProperties[i].arraySize;
                    if (fieldProperties[i].isExpanded) rect.y += EditorGUIUtility.singleLineHeight * 2;
                }
            }
        }
    }

    // must manually set the height of the container we are drawing to 
    // much easier since we use reflection
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int numProperties = 1;  // 5 properties +2 for description text area
        Init(property);
       
        if (optionType_prop != null)
        {
            OptionType optionType = (OptionType)optionType_prop.enumValueIndex;
            for (int i = 0; i < fieldProperties.Length; i++)
            {
                if (fieldProperties[i] == null) continue;
                if (fields[i].GetCustomAttribute<HideInInspector>() != null || fields[i].GetCustomAttribute<SkipFieldAttribute>() != null) continue;

                var typeAttrib = fields[i].GetCustomAttribute<OptionTypeAttribute>();
                if (typeAttrib == null || ElementInputOptionData.OptionTypeComptaible(optionType, typeAttrib.Type))
                {
                    numProperties++;
                    if (fieldProperties[i].isArray)
                    {
                        numProperties += fieldProperties[i].arraySize;  
                        if (fieldProperties[i].isExpanded) numProperties += 3;  // + 3 for the +- button and gap underneath
                    }
                }
            }
        }
        return EditorGUIUtility.singleLineHeight * numProperties;
    }
}
