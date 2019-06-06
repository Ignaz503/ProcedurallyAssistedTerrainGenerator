using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(ChunkManager))]
public class ChunkManagerEditor : Editor
{
    ChunkManager manager;
    public void OnSceneGUI()
    {
        Tools.hidden = false;
        manager = target as ChunkManager;

        HandleInitialCreation();

        for (int i = 0; i < manager.ToManage.Chunks.Count; i++)
        {
            var cTM = manager.ToManage.Chunks[i];

            var chunkHandle = new ChunkHandle() { ChunkToHandle = cTM.chunk, Editor = this };

            chunkHandle.DrawChunkHandle();

        }
        Tools.hidden = true;
    }

    private void HandleInitialCreation()
    {
        if (manager.ToManage.Chunks.Count == 0)
        {
            //add defualt chunk
            AddNewChunk(Vector3.zero);
        }
    }

    public bool ContainsChunkByCoord(Vector3 coord)
    {
        return manager.ContainsChunkByCoord(coord);
    }

    public void AddNewChunk(Vector3 pos)
    {
        manager.AddChunk(pos);
    }
}

public struct ChunkHandle
{
    public static float HandleSize = 2f;
    public static float PotentialSize { get { return HandleSize * .25f; } }

    public Chunk ChunkToHandle;
    public ChunkManagerEditor Editor;

    Vector3 Center
    {
        get
        {
            Vector3 chunkNormViaExtends = ChunkToHandle.Center ;
            Vector3 ext = ChunkToHandle.Extents;

            chunkNormViaExtends.Scale(new Vector3(
                1.0f / ext.x,
                1.0f / ext.y,
                1.0f / ext.z
                ));

            return chunkNormViaExtends * HandleSize;
        }
    }

    public void DrawChunkHandle()
    {
        Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
        Handles.color = Color.Lerp(Color.black, Color.gray, .5f);
        Handles.CubeHandleCap(0, Center, Quaternion.identity, HandleSize, EventType.Repaint);

        DrawPotentialNeighbors();

    }

    private void DrawPotentialNeighbors()
    {
        Vector3 coords = ChunkToHandle.Center;
        //x neighbors
        DrawPotentialNeighborsForAxis(coords, Vector3.right * HandleSize);

        //y neighbors
        DrawPotentialNeighborsForAxis(coords,Vector3.up * HandleSize);


        //z neighbors
        DrawPotentialNeighborsForAxis(coords,Vector3.forward * PotentialSize);

    }

    void DrawPotentialNeighborsForAxis(Vector3 coords, Vector3 dir)
    {
        if (!Editor.ContainsChunkByCoord(coords + dir))
        {
            DrawPotentialHandle(coords + dir);
        }

        if (!Editor.ContainsChunkByCoord(coords - dir))
        {
            DrawPotentialHandle(coords - dir);
        }
    }

    void DrawPotentialHandle(Vector3 pos)
    {
        Handles.color = Color.Lerp(Color.yellow, Color.white, .25f);
        if (Handles.Button(pos, Quaternion.identity, PotentialSize, PotentialSize, Handles.CubeHandleCap))
        {
            //add new chunk
            pos = pos.normalized;
            pos.Scale(Chunk.DefaultExtents);
            Debug.Log("Add new chunk");
            Editor.AddNewChunk(pos);
        }
    }

}