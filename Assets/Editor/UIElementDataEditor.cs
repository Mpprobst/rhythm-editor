using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Unity.VisualScripting;

[CustomEditor(typeof(UIElementData)), CanEditMultipleObjects]
public class UIElementDataEditor : Editor
{
    public SerializedProperty elementName_prop, elementType_prop, elementPrefab_prop, popoutPrefab_prop;
    private SerializedProperty[] fieldProperties;

    private FieldInfo[] fields;

    private void OnEnable()
    {
        elementName_prop = serializedObject.FindProperty("elementName");
        elementType_prop = serializedObject.FindProperty("elementType");
        elementPrefab_prop = serializedObject.FindProperty("elementPrefab");
        popoutPrefab_prop = serializedObject.FindProperty("popoutPrefab");

        fields = typeof(UIElementData).GetFields(BindingFlags.Public | BindingFlags.Instance);
        fieldProperties = new SerializedProperty[fields.Length];
        
        for (int i = 0; i < fields.Length; i++)
        {
            if (fields[i].GetCustomAttribute<HideInInspector>() != null || fields[i].GetCustomAttribute<SkipFieldAttribute>() != null) continue;
            fieldProperties[i] = serializedObject.FindProperty(fields[i].Name);
        }
    }

    // if you don't use the serialized properties, your changes in the inspector window will not apply to your object!
    // ex: you instead get the class instance and set UIElementData.elementName directly with your EditorGUIs will not work.
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(elementType_prop);

        UIElementType elementType = (UIElementType)elementType_prop.enumValueIndex;
        string elementPrefabName = $"UI/Elements/UIElement_{Utils.ToHumanReadable(elementType)}";
        if (elementType == UIElementType.POPOUT)
        {
            elementPrefabName += "Button";

            string popoutName = $"UI/Popouts/{elementName_prop.stringValue}_Popout";
            Object popoutPrefab = Resources.Load<Object>(popoutName);
            if (popoutPrefab == null && popoutPrefab_prop.objectReferenceValue)
                popoutPrefab = Resources.Load<Object>("UI/Popouts/PopoutBase");
            popoutPrefab_prop.objectReferenceValue = popoutPrefab; 
        }
        elementPrefab_prop.objectReferenceValue = Resources.Load(elementPrefabName);

        // reflection here is way more scalable, especially if we add new fields to our class later
        // otherwise we are finding 
        for (int i = 0; i < fieldProperties.Length; i++)
        {
            if (fieldProperties[i] == null) continue;
            if (fields[i].GetCustomAttribute<HideInInspector>() != null || fields[i].GetCustomAttribute<SkipFieldAttribute>() != null) continue;

            var typeAttrib = fields[i].GetCustomAttribute<ElementTypeAttribute>();
            if (typeAttrib == null || typeAttrib.ElementType <= elementType)
            {
                // draw the property
                EditorGUILayout.PropertyField(fieldProperties[i]);
            }
        }

        serializedObject.ApplyModifiedProperties();

    }

}
