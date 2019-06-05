using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class ChunkSceneView : SceneView
{
    List<string> prevOpenScenePahs;
    public ChunkManager manager;

    public void Awake()
    {
        prevOpenScenePahs = new List<string>();

        //save all active scenes
        //open a new one with name something bs
    }

    public new void OnDestroy()
    {
        base.OnDestroy();
    }

    public new void OnGUI()
    {
        base.OnGUI(); 

        //close current scene and destoy i guess

        //reload all old scenes

    }

    public void SetChunkManager(ChunkManager m)
    {
        manager = m;
    }

    void GetActiveScenesAndTempStore()
    {
        prevOpenScenePahs.Clear();
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