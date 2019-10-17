using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainGenBaseSettings),true)]
public class TerrainGenBaseSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var settings = target as TerrainGenBaseSettings;
        base.OnInspectorGUI();
        if (GUILayout.Button("Change Default Workspace"))
        {
            string path = EditorUtility.SaveFolderPanel("Select Workspace", "", "");

            if (path.Length != 0)
            {
                string p = path.Remove(0, Application.dataPath.Length);
                settings.Workspace = p;
                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
            }
        }

        if (GUILayout.Button("Set Blender Path"))
        {
            string path = EditorUtility.OpenFilePanel("Find Blender Executable", "", "exe");
            if (path.Length != 0)
            {
                settings.PathToBlender = path;
                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
            }
        }

        if (GUILayout.Button("Save"))
        {
            AssetDatabase.ForceReserializeAssets(new string[] { AssetDatabase.GetAssetPath(target) });
        }
    }
}
