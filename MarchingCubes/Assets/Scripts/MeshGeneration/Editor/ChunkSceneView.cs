using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System;

public class ChunkSceneView : SceneView
{
    List<string> prevOpenScenePahs;
    public ChunkManager manager;
    Scene openScene;
    TerrainChunkWindow callingWindow;
    GameObject chunkObject;

    public void Initialize(TerrainChunkWindow callingWindow)
    {

        prevOpenScenePahs = new List<string>();

        //save all active scenes
        //open a new one with name something bs
        GetActiveScenesAndTempStore();
        this.callingWindow = callingWindow;
        CreateScene();
        Tools.hidden = true;
    }

    void CreateScene()
    {
        openScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CenterView();

        GameObject obj = new GameObject();
        obj.name = "Chunks Holder";
        manager = obj.AddComponent<ChunkManager>();


        //add light
        GameObject lightobj = new GameObject();
        lightobj.name = "Light";
        lightobj.transform.position = new Vector3(0, 100, 0);
        lightobj.transform.rotation = Quaternion.Euler(new Vector3(50, -30, 0));

        var light = lightobj.AddComponent<Light>();

        light.color = Color.white;
        light.type = LightType.Directional;

        Lightmapping.Bake();

        //mark gameobject in scene hierarchy
        Selection.activeObject = obj;

        chunkObject = obj;

    }

    void CenterView()
    {
        pivot = Vector3.up * 10;
        rotation = Quaternion.Euler(90, 0, 0);
    }

    public new void OnDestroy()
    {
        //inform  calling window of new chunks list
        callingWindow.UpdateChunksToManage(manager.ToManage);

        //reload all old scenes
        ReopenPreviousScenes();

        //close current scene and destoy i guess
        EditorSceneManager.CloseScene(openScene,true);
        
        Tools.hidden = false;
        base.OnDestroy();
    }
    
    public new void OnGUI()
    {
        base.OnGUI();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Ahh the chunks are missing, please make them be drawn again"))
        {
            Selection.activeObject = chunkObject;
        }

        if (GUILayout.Button("Recenter me, i'm lost"))
        {
            CenterView();
        }

        if (GUILayout.Button("Close"))
        {
            Close();
        }
        EditorGUILayout.EndHorizontal();

    }

    public void SetChunksToManage(ChunksToManage m)
    {
        manager.ToManage = m;
        manager.CreateChunks();
    }

    void GetActiveScenesAndTempStore()
    {
        prevOpenScenePahs.Clear();
        EditorSceneManager.SaveOpenScenes();
        int countLoaded = EditorSceneManager.sceneCount;

        for (int i = 0; i < countLoaded; i++)
        {
            var sc = EditorSceneManager.GetSceneAt(i);
            prevOpenScenePahs.Add(sc.path);
        }
    }

    public void ReopenPreviousScenes()
    {
        foreach (var path in prevOpenScenePahs)
        {
            EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
        }
    }

}