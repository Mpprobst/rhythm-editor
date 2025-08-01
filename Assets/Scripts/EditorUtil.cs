using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorUtil 
{
    public static void SaveObjectAsPrefab(GameObject prefabToSave, string savePath)
    {
#if UNITY_EDITOR
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
#endif
    }
}
