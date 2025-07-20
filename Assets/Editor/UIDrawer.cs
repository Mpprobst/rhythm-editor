using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// a custom window that organizes the UI and generates it with a buton
public class UIDrawer : EditorWindow
{
    // this is just the window. functionality is in a different static class so we can do it at runtime and in editor

    // properties
    public SerializedProperty uiLayout_Prop;
    public SerializedObject seralizedObject;


    // internal variables
    public LayoutData toolbarLayout;
    private string layoutFolder = "Assets/Resources/Prefabs/UI/Layouts";
    private string popupsFolder = "Assets/Resources/Prefabs/UI/Popups";
    public string layoutName = "UI";

    private string toolbarBasePrefabPath = "Prefabs/UI/LayoutBase";

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(UIDrawer));
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

        GUILayout.Label("Toolbar Settings", EditorStyles.boldLabel);
        layoutName = EditorGUILayout.TextField("Layout Name", layoutName);

        EditorGUILayout.PropertyField(uiLayout_Prop, true);
        seralizedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Create UI"))
        {
            GameObject toolbarObj = GameObject.Find(layoutName);

            if (toolbarObj == null)
            {
                // create new scene instance from prefab if saved
                string existingPrefabPath = $"{layoutFolder}/{layoutName}.prefab";
                if (File.Exists(existingPrefabPath))
                {
                    Debug.Log($"Loading existing prefab {existingPrefabPath}");
                    toolbarObj = (GameObject)PrefabUtility.InstantiatePrefab(Resources.Load(existingPrefabPath));
                }
            }

            Debug.Log($"Creating layout {layoutFolder}/{layoutName}");
            if (CreateLayout(ref toolbarObj))
            {
                SaveObjectAsPrefab(toolbarObj, layoutFolder);
            }
            else if (toolbarObj != null)
            {
                DestroyImmediate(toolbarObj);
            }
        }
    }

    private void SaveObjectAsPrefab(GameObject prefabToSave, string savePath)
    {
        if (prefabToSave == null) return;

        if (!Directory.Exists(savePath))
        {
            int lastFolder = savePath.LastIndexOf('/');
            string parentFolder = savePath.Substring(0, lastFolder);
            string newFolder = savePath.Substring(lastFolder + 1, savePath.Length - lastFolder - 1);
            Debug.Log($"create folder {parentFolder}/{newFolder}");
            AssetDatabase.CreateFolder(parentFolder, newFolder);
        }

        string localPath = $"{savePath}/{prefabToSave.name}.prefab";
        bool saveSuccess;
        PrefabUtility.SaveAsPrefabAssetAndConnect(prefabToSave, localPath, InteractionMode.AutomatedAction, out saveSuccess);
        PrefabUtility.RevertPrefabInstance(prefabToSave, InteractionMode.AutomatedAction);

        if (saveSuccess)
        {
            Debug.Log($"Saved {localPath}");
        }
        else
        {
            Debug.LogWarning($"Failed to save new prefab at {localPath}");
        }
    }

    private bool CreateLayout(ref GameObject layoutObj)
    {
        return true;
    }
}
