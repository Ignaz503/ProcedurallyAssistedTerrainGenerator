﻿using System;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class TerrainChunkWindow : EditorWindow
{
    public enum DensityFuncApplyMode
    {
        OneForAll,
        PerChunk
    }


    [MenuItem("ProcAssistTerrainGen/Cube March Terrain Editor")]
    private static void OpenWindow()
    {
        TerrainChunkWindow window = GetWindow<TerrainChunkWindow>();

        window.titleContent = new GUIContent("Terrain Chunk Editor");
        window.Initialize();
    }

    [SerializeField] TerrainGenBaseSettings settings = null;

    int chunkResolution;

    DensityFuncApplyMode densityFuncApplyMode;
    string functionTypeString;
    Type densityFunctionType;

    ChunksToManage chunksToManage;

    bool show;

    public void Initialize()
    {
        chunkResolution = settings.MinChunkResolution;
        densityFuncApplyMode = DensityFuncApplyMode.OneForAll;

        show = true;

        if (Chunk.DefaultExtents.sqrMagnitude <= 0)
            Chunk.DefaultExtents = Vector3.one * 5f;


        chunksToManage = new ChunksToManage();
        chunksToManage.Chunks = new List<ChunkToGenerate>();
        chunksToManage.AddChunk(Vector3.zero);

    }

    public void UpdateChunksToManage(ChunksToManage toManage)
    {
        chunksToManage = toManage;
    }

    public void OnGUI()
    {
        DrawWithLayout();
    }


    private void DrawWithLayout()
    {
        Rect r = EditorGUILayout.BeginVertical();

        DrawGeneralInfoArea();
        EditorGUIExtensions.Space(5);
        DrawChunkFormCreator();
        EditorGUIExtensions.Space(5);
        DrawFooter();

        EditorGUILayout.BeginVertical();
    }

    private void DrawChunkFormCreator()
    {
        var style = EditorStyles.largeLabel;
        style.fontStyle = FontStyle.Bold;
        EditorGUILayout.LabelField(new GUIContent("Terrain Form:"), style);

        EditorGUIExtensions.Space(2);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Edit Chunks"))
        {
            if (EditorUtility.DisplayDialog("Start Editing Chunks", "This is going to open a new scene and close all currently open ones (saving them beforehand). Make sure there are no unwanted changes that would get saved in any open scene.", "Proceed"))
            {
                var sv =  GetWindow<ChunkSceneView>();
                sv.Initialize(this);

                sv.SetChunksToManage(chunksToManage);
                sv.Focus();
                Docker.Dock(this, sv, Docker.DockPosition.Bottom);
            }
            
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawGeneralInfoArea()
    {
        var style = EditorStyles.largeLabel;
        style.fontStyle = FontStyle.Bold;
        EditorGUILayout.LabelField(new GUIContent("General Settings"), style);

        EditorGUIExtensions.Space(3);

        EditorGUILayout.BeginVertical();

        DrawChunkSettings();

        EditorGUIExtensions.Space(3);

        DrawDensityFunctionChooser();
        
        EditorGUILayout.EndVertical();
    }

    private void DrawDensityFunctionChooser()
    {
        EditorGUILayout.LabelField(new GUIContent("Density Function Settings: "), EditorStyles.boldLabel);

        EditorGUIExtensions.Space(2);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Density Function Apply Mode: "));
        densityFuncApplyMode = (DensityFuncApplyMode)EditorGUILayout.EnumPopup(densityFuncApplyMode) ;
        EditorGUILayout.EndHorizontal();

        if (densityFuncApplyMode == DensityFuncApplyMode.OneForAll)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Function: ");
            string buttonString = densityFunctionType != null ? densityFunctionType.ToString() : "Chose";
            if (GUILayout.Button(buttonString, EditorStyles.popup))
            {
                Rect r = EditorGUILayout.GetControlRect();
                r.position = Event.current.mousePosition;
                DrawDensityFuncChooser(r);
            }
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            show = EditorGUILayout.Foldout(show, $"Chunks: ");
            if (show)
            {
                for (int i = chunksToManage.Chunks.Count -1 ; i >= 0 ; i--)
                {
                    var chunkInfo = chunksToManage.Chunks[i];

                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField($"Function For: {chunkInfo.chunk.Center}");

                    if (chunksToManage.Chunks.Count > 1)
                    {
                        if (GUILayout.Button("x", EditorStyles.label))
                        {
                            chunksToManage.RemoveChunkAt(i,true);
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    string buttonString = chunkInfo.DensityFunc != null ? chunkInfo.DensityFunc.ToString() : "Chose";

                    if (GUILayout.Button(buttonString, EditorStyles.popup))
                    {
                        Rect r = EditorGUILayout.GetControlRect();
                        r.position = Event.current.mousePosition;
                        DrawDensityFuncChooser(r, chunkInfo);
                    }

                    EditorGUILayout.EndVertical();
                }
            }
        }
    }

    void DrawChunkSettings()
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField(new GUIContent("Chunk Settings: "), EditorStyles.boldLabel);

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Chunk Sample Resolution: "));
        chunkResolution = EditorGUILayout.IntSlider(chunkResolution, settings.MinChunkResolution, settings.MaxChunkResolution);
        EditorGUILayout.EndHorizontal();

        Chunk.DefaultExtents = EditorGUILayout.Vector3Field(new GUIContent("Base Chunk Extents (extends are half bounds size): "), Chunk.DefaultExtents);

        EditorGUILayout.EndVertical();
    }

    private void DrawFooter()
    {
        if (GUILayout.Button("Generate"))
        {
            //TODO Generate

            if (PossibleToGenerate())
            {
                Debug.Log("Possible to generate, only implementation is left^^");
            }

        }
    }

    private bool PossibleToGenerate()
    {
        switch (densityFuncApplyMode)
        {
            case DensityFuncApplyMode.OneForAll:
                return CheckIfOneForAllApplyOK();
            case DensityFuncApplyMode.PerChunk:
                return CheckIfPerChunkApllyOk();
            default:
                Debug.LogError("Unknown Density Function Apply Mode");
                return false;
        }
    }

    private bool CheckIfPerChunkApllyOk()
    {
        for (int i = 0; i < chunksToManage.Chunks.Count; i++)
        {
            if (chunksToManage.Chunks[i].DensityFunc == null)
            {
                Debug.Log($"Chunk {i} (coords: {chunksToManage.Chunks[i].chunk.Center}) still needs a density function to be chosen");
                return false;
            }
        }
        return true;
    }

    private bool CheckIfOneForAllApplyOK()
    {
        if (densityFunctionType != null) 
            return true;

        Debug.Log("Please chose a density function before generating");
        return false;
    }

    void DrawDensityFuncChooser(Rect r)
    {

        //Get all density functions
        var assem = Assembly.GetAssembly(typeof(IDensityFunc));
        var allPossFuncTypes = assem.GetTypes().Where(t => typeof(IDensityFunc).IsAssignableFrom(t) && t != typeof(IDensityFunc));

        //generic menu shown as dropdown
        GenericMenu menu = new GenericMenu();

        foreach (var funcType in allPossFuncTypes)
        {
            menu.AddItem(new GUIContent(funcType.ToString()), false, () => SetFunctionType(funcType));
        }

        menu.DropDown(r);
    }

    void DrawDensityFuncChooser(Rect r, ChunkToGenerate chunk)
    {
        //Get all density functions
        var assem = Assembly.GetAssembly(typeof(IDensityFunc));
        var allPossFuncTypes = assem.GetTypes().Where(t => typeof(IDensityFunc).IsAssignableFrom(t) && t != typeof(IDensityFunc));

        //generic menu shown as dropdown
        GenericMenu menu = new GenericMenu();

        foreach (var funcType in allPossFuncTypes)
        {
            var type = funcType;//closure safety
            menu.AddItem(new GUIContent(funcType.ToString()), false, () => chunk.SetDensityFunction(type));
        }

        menu.DropDown(r);
    }


    void SetFunctionType(Type t)
    {
        densityFunctionType = t;
    }
}




