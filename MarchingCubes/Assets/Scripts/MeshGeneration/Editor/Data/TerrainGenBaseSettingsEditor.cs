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

        EditorGUILayout.LabelField(nameof(settings.MinChunkResolution));
        settings.MinChunkResolution = EditorGUILayout.IntField(settings.MinChunkResolution);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField(nameof(settings.MaxChunkResolution));
        settings.MaxChunkResolution = EditorGUILayout.IntField(settings.MaxChunkResolution);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField(nameof(settings.Workspace));
        EditorGUILayout.LabelField(settings.Workspace);
        if (GUILayout.Button("Change Default Workspace"))
        {
            string path = EditorUtility.SaveFolderPanel("Select Workspace", "", "");
            
            if (path.Length != 0)
            {
                settings.Workspace = path.Remove(0,Application.dataPath.Length);
            }
        }
    }
}
