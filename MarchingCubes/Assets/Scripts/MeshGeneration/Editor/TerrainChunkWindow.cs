using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class TerrainChunkWindow : EditorWindow
{

    [MenuItem("Window/Cube March Terrain Editor")]
    private static void OpenWindow()
    {
        TerrainChunkWindow window = GetWindow<TerrainChunkWindow>();

        window.titleContent = new GUIContent("Terrain Chunk Editor");
        window.Initialize();
    }

    public void Initialize()
    {

    }

    public void OnGUI()
    {
        DrawWithLayout();
    }

    private void DrawWithLayout()
    {
        Rect r = EditorGUILayout.BeginVertical();
        
        EditorGUILayout.BeginVertical();
    }

    private void DrawChunkFormCreator(Rect baseRect)
    {
        EditorGUILayout.LabelField(new GUIContent("General Info"));
    }

    private void DrawGeneralInfoArea(Rect baseRect)
    {
        EditorGUILayout.LabelField(new GUIContent("Chunk Form Creator"));
    }
}
