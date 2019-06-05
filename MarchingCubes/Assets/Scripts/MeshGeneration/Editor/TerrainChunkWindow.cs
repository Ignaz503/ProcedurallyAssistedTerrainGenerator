using System;
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
    Type functionType;

    ChunksToManage chunksToManage;

    bool show;

    public void Initialize()
    {
        chunkResolution = settings.MinChunkResolution;
        densityFuncApplyMode = DensityFuncApplyMode.OneForAll;

        chunksToManage = new ChunksToManage();
        chunksToManage.Chunks = new List<ChunkToGenerate>();

        show = true;
        
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
        DrawChunkFormCreator();
        DrawFooter();

        EditorGUILayout.BeginVertical();
    }

    private void DrawChunkFormCreator()
    {
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
        EditorGUILayout.LabelField(new GUIContent("General Info"), EditorStyles.largeLabel);
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField(new GUIContent("Settings:"),EditorStyles.boldLabel);
        EditorGUILayout.Space();

        DrawResolutionChooser();

        EditorGUILayout.Space();

        DrawDensityFunctionChooser();

        EditorGUILayout.EndVertical();
    }

    private void DrawDensityFunctionChooser()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Density Function Apply Mode: "));
        densityFuncApplyMode = (DensityFuncApplyMode)EditorGUILayout.EnumPopup(densityFuncApplyMode) ;
        EditorGUILayout.EndHorizontal();

        if (densityFuncApplyMode == DensityFuncApplyMode.OneForAll)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Function: ");
            string buttonString = functionType != null ? functionType.ToString() : "Chose";
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
                foreach(var chunkInfo  in chunksToManage.Chunks)
                {
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField($"Function For: {chunkInfo.chunk.Center}");
                    string buttonString = chunkInfo.DensityFunc != null ? chunkInfo.DensityFunc.ToString() : "Chose";
                    var c = chunkInfo;//just to be save with closure
                    if (GUILayout.Button(buttonString, EditorStyles.popup))
                    {
                        Rect r = EditorGUILayout.GetControlRect();
                        r.position = Event.current.mousePosition;
                        DrawDensityFuncChooser(r, c);
                    }

                    EditorGUILayout.EndVertical();
                }
            }
        }
    }

    void DrawResolutionChooser()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Chunk Sample Resolution: "));
        chunkResolution = EditorGUILayout.IntSlider(chunkResolution, settings.MinChunkResolution, settings.MaxChunkResolution);
        EditorGUILayout.EndHorizontal();
    }

    private void DrawFooter()
    {
        if (GUILayout.Button("Generate"))
        {
            //TODO Generate
            Debug.Log("Generate not implemented");
        }
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
        var allPossFuncTypes = Assembly.GetAssembly(typeof(IDensityFunc)).GetTypes().Where(t => t.IsAssignableFrom(typeof(IDensityFunc)));

        //generic menu shown as dropdown
        GenericMenu menu = new GenericMenu();

        foreach (var funcType in allPossFuncTypes)
        {
            menu.AddItem(new GUIContent(funcType.ToString()), false, () => chunk.SetDensityFunction(funcType));
        }

        menu.DropDown(r);
    }


    void SetFunctionType(Type t)
    {
        functionType = t;
    }
}




