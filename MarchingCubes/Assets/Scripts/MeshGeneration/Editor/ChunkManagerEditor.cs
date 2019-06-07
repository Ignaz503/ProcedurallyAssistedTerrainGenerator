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

        for (int i = manager.ToManage.Chunks.Count - 1; i >= 0 ; i--)
        {
            var cTM = manager.ToManage.Chunks[i];

            var chunkHandle = new ChunkHandle() { ChunkToHandle = cTM.Chunk, Editor = this };

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

    public void RemoveChunk(Chunk chunk)
    {
        manager.RemoveChunk(chunk);
    }
}

public struct ChunkHandle
{
    public static float HandleSize = 2f;
    public static float PotentialSize { get { return HandleSize * .25f; } }

    public Chunk ChunkToHandle;
    public ChunkManagerEditor Editor;

    Vector3 CenterLocal
    {
        get
        {
            Vector3 chunkNormViaExtends = ChunkToHandle.Center ;
            Vector3 ext = ChunkToHandle.Extents;

            chunkNormViaExtends.Scale(new Vector3(
                1.0f / (2f*ext.x),
                1.0f / (2f * ext.y),
                1.0f / (2f * ext.z)
                ));

            return chunkNormViaExtends * HandleSize;
        }
    }

    Vector3 Extents
    {
        get
        {
            return ChunkToHandle.Extents;
        }
    }

    Vector3 Center
    {
        get
        {
            return ChunkToHandle.Center;
        }
    }
    
    public void DrawChunkHandle()
    {
        Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
        Handles.color = Color.Lerp(Color.black, Color.gray, .5f);
        if (Event.current.modifiers == EventModifiers.Control)
        {
            if (Handles.Button(CenterLocal, Quaternion.identity, HandleSize, HandleSize, Handles.CubeHandleCap))
            {
                //right click
                Editor.RemoveChunk(ChunkToHandle);
            }

        }
        else
        {
            Handles.CubeHandleCap(0, CenterLocal, Quaternion.identity, HandleSize, EventType.Repaint);

            DrawPotentialNeighbors();
        }

    }

    private void DrawPotentialNeighbors()
    {
        Vector3 coords = CenterLocal;
        //x neighbors
        DrawPotentialNeighborsForAxis(coords, Vector3.right * HandleSize);

        //y neighbors
        DrawPotentialNeighborsForAxis(coords,Vector3.up * HandleSize);


        //z neighbors
        DrawPotentialNeighborsForAxis(coords,Vector3.forward * HandleSize);

    }

    void DrawPotentialNeighborsForAxis(Vector3 coords, Vector3 dir)
    {
        Vector3 pos = coords + dir;
        if (!Editor.ContainsChunkByCoord(MakeLocalToWorldChunkCoord(pos)))
        {
            DrawPotentialHandle(pos);
        }

        pos = coords - dir;
        if (!Editor.ContainsChunkByCoord(MakeLocalToWorldChunkCoord(pos)))
        {
            DrawPotentialHandle(pos);
        }
    }

    Vector3 MakeLocalToWorldChunkCoord(Vector3 coord)
    {
        //add new chunk
        coord.Scale(new Vector3(1.0f / HandleSize, 1.0f / HandleSize, 1.0f / HandleSize));
        coord.Scale(2*Chunk.DefaultExtents);
        return coord;
    }

    void DrawPotentialHandle(Vector3 pos)
    {
        Handles.color = Color.Lerp(Color.yellow, Color.white, .25f);
        if (Handles.Button(pos, Quaternion.identity, PotentialSize, PotentialSize, Handles.CubeHandleCap))
        {
            pos = MakeLocalToWorldChunkCoord(pos);
            Editor.AddNewChunk(pos);
        }
    }

}