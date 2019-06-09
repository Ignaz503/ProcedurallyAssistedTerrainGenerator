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
    Type densityFunctionType;

    bool flatShaded = true;

    ChunksToManage chunksToManage;
    SurfaceGenerator generator;
    Transform terrainRoot;

    bool show;
    bool generating = false;
    int toGenerate;
    int alreadyDone;
    int update = 0;

    public void Initialize()
    {
        chunkResolution = settings.MinChunkResolution;
        densityFuncApplyMode = DensityFuncApplyMode.OneForAll;

        show = true;

        if (Chunk.DefaultExtents.sqrMagnitude <= 0)
            Chunk.DefaultExtents = Vector3.one * 10f;


        chunksToManage = new ChunksToManage();
        chunksToManage.Chunks = new List<Generate>();
        chunksToManage.AddChunk(Vector3.zero);
        TagHelper.AddTag("TerrainRoot");
        FindOrCreateSurfaceGenerator();
    }

    void FindOrCreateSurfaceGenerator()
    {
        var generator = FindObjectOfType<SurfaceGenerator>();

        if (generator == null)
        {
            //need to create surface generator
            Debug.Log("Need to create surface generator, will be an editor only gameobject in your current scene");
            GameObject gen = new GameObject();
            gen.name = "Surface Generator";
            gen.tag = "EditorOnly";
            gen.transform.position = Vector3.zero;
            generator = gen.AddComponent<SurfaceGenerator>();

        }
        this.generator = generator;
        generator.OnChunkGenreated += OnChunkGenerated;
        terrainRoot = generator.Terrain;
        if (!generator.IsUseableInEditor)
            generator.MakeUseableInEditor();
    }

    public void UpdateChunksToManage(ChunksToManage toManage)
    {
        chunksToManage = toManage;
    }

    public void OnGUI()
    {
        DrawWithLayout();
    }

    void OnChunkGenerated(Chunk c)
    {
        alreadyDone++;
        if (alreadyDone >= toGenerate)
        {
            generating = false;
        }
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
                var sv =  GetWindow<ChunkSceneView>(typeof(TerrainChunkWindow));
                sv.Initialize(this);

                sv.SetChunksToManage(chunksToManage);
                sv.Focus();
                sv.Show();
                //Docker.Dock(this, sv, Docker.DockPosition.Bottom);
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
        DrawChunks();
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

        if (GUILayout.Button("Create New Denisty Function"))
        {
            EditorWindow window = GetWindow<FunctionGraphEditor>(typeof(TerrainChunkWindow));
            window.Show();

        }

    }

    void DrawChunks()
    {
        show = EditorGUILayout.Foldout(show, $"Chunks: ");
        if (show)
        {
            for (int i = chunksToManage.Chunks.Count - 1; i >= 0; i--)
            {
                var chunkInfo = chunksToManage.Chunks[i];

                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField($"Chunk at: {chunkInfo.Chunk.Center}");

                if (chunksToManage.Chunks.Count > 1)
                {
                    if (GUILayout.Button("x", EditorStyles.label))
                    {
                        chunksToManage.RemoveChunkAt(i, true);
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (densityFuncApplyMode == DensityFuncApplyMode.PerChunk)
                {
                    string buttonString = chunkInfo.DensityFunc != null ? chunkInfo.DensityFunc.ToString() : "Chose";
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Function");

                    if (GUILayout.Button(buttonString, EditorStyles.popup))
                    {
                        Rect r = EditorGUILayout.GetControlRect();
                        r.position = Event.current.mousePosition;
                        DrawDensityFuncChooser(r, chunkInfo);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
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

        Vector3 old = Chunk.DefaultExtents;
        Chunk.DefaultExtents = EditorGUILayout.Vector3Field(new GUIContent("Base Chunk Extents (extends are half bounds size): "), Chunk.DefaultExtents);

        if ((old - Chunk.DefaultExtents).sqrMagnitude > 0)
        {
            //we have new extents
            chunksToManage.UpdateExtents(Chunk.DefaultExtents);
        }

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Flat Shading: ");
        flatShaded = EditorGUILayout.Toggle(flatShaded);

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private void DrawFooter()
    {
        if (GUILayout.Button("Generate"))
        {
            //TODO Generate

            if (PossibleToGenerate())
            {
                StartTrackingProcess();
                Generate();
            }
        }
        if (generating)
            DisplayGeneratingProgress();
    }

    void DisplayGeneratingProgress()
    {
        update++;
        EditorGUILayout.LabelField($"Generating {GeneratingDots()}");
        Repaint();
    }

    string GeneratingDots()
    {
        int val = update / 20;
        string s = "";
        for (int i = 0; i < val % 4; i++)
        {
            s += ".";
        }
        return s;
    }

    private void StartTrackingProcess()
    {
        generating = true;
        toGenerate = chunksToManage.Chunks.Count;
        alreadyDone = 0;
        update = 0;
    }

    private void Generate()
    {
        switch (densityFuncApplyMode)
        {
            case DensityFuncApplyMode.OneForAll:
                generator.RequestMesh(chunksToManage.Chunks, densityFunctionType, chunkResolution,flatShaded);
                break;
            case DensityFuncApplyMode.PerChunk:
                generator.RequestMesh(chunksToManage.Chunks, chunkResolution, flatShaded);
                break;
            default:
                Debug.LogError("Unknonw Density Function Apply mode");
                break;
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
                Debug.Log($"Chunk {i} (coords: {chunksToManage.Chunks[i].Chunk.Center}) still needs a density function to be chosen");
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

    void DrawDensityFuncChooser(Rect r, Generate chunk)
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




