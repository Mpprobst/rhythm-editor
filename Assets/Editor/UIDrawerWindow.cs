using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// a custom window that organizes the UI and generates it with a buton
public class UIDrawerWindow : EditorWindow
{
    // this is just the window. functionality is in a different static class so we can do it at runtime and in editor
    public LayoutData uiLayout;

    // properties
    public SerializedProperty uiLayout_Prop;
    public SerializedObject seralizedObject;


    // internal variables
    public LayoutData layoutData;
    private string layoutFolder = "Assets/Resources/UI/Layouts";
    private string popupsFolder = "Assets/Resources/UI/Popups";
    public string layoutName = "UI";

    private string toolbarBasePrefabPath = "UI/LayoutBase";
    [MenuItem("Window/UI Drawer")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(UIDrawerWindow));
    }

    private void OnEnable()
    {
        // The actual window code goes here
        ScriptableObject target = this;
        seralizedObject = new SerializedObject(target);
        uiLayout_Prop = seralizedObject.FindProperty("uiLayout");
    }

    void OnGUI()
    {
        seralizedObject.Update();

        GUILayout.Label("Layout Settings", EditorStyles.boldLabel);
        layoutName = EditorGUILayout.TextField("Layout Name", layoutName);

        EditorGUILayout.PropertyField(uiLayout_Prop, true);
        seralizedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Create UI"))
        {
            GameObject layoutObj = GameObject.Find(layoutName);
            UILayout layout = null;
            if (layoutObj)
                layout = layoutObj.GetComponent<UILayout>();

            LayoutGenerator.CreateLayout(uiLayout_Prop.objectReferenceValue as LayoutData, ref layout);
            layout.name = layoutName;
            /*
            if (layoutObj == null)
            {
                // create new scene instance from prefab if saved
                string existingPrefabPath = $"{layoutFolder}/{layoutName}.prefab";
                if (File.Exists(existingPrefabPath))
                {
                    Debug.Log($"Loading existing prefab {existingPrefabPath}");
                }
                else
                {
                    existingPrefabPath = toolbarBasePrefabPath;
                }
                layoutObj = (GameObject)PrefabUtility.InstantiatePrefab(Resources.Load(existingPrefabPath));
            }
            
            Debug.Log($"Creating layout {layoutFolder}/{layoutName}");
            if (CreateLayout(ref layoutObj))
            {
                SaveObjectAsPrefab(layoutObj, layoutFolder);
            }
            else if (layoutObj != null)
            {
                DestroyImmediate(layoutObj);
            }*/
        }
    }



    private bool CreateLayout(ref GameObject layoutObj)
    {
        UILayout layout = null;
        if (layoutObj) layout = layoutObj.GetComponent<UILayout>();
        LayoutGenerator.CreateLayout(layoutData, ref layout);
        EditorUtil.SaveObjectAsPrefab(layout.gameObject, "Assets/Resources/UI/Layouts");

        return true;
    }
}
