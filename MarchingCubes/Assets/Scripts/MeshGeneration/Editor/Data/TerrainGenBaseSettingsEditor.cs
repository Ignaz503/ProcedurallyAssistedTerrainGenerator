using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainGenBaseSettings),true)]
public class TerrainGenBaseSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Change Default Workspace"))
        {
            string path = EditorUtility.SaveFolderPanel("Select Workspace", "", "");

            if (path.Length != 0)
            {
                string p = path.Remove(0, Application.dataPath.Length);
                (target as TerrainGenBaseSettings).Workspace = p;
            }
        }
        if (GUILayout.Button("Save"))
        {
            AssetDatabase.ForceReserializeAssets(new string[] { AssetDatabase.GetAssetPath(target) });
        }
    }
}
