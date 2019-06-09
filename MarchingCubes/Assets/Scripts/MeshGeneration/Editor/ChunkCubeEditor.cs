using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(ChunkCube))]
public class ChunkCubeEditor : Editor
{
    ChunkCube tar;

    public void OnSceneGUI()
    {

        tar = target as ChunkCube;

        if (Event.current.isKey && Event.current.keyCode == KeyCode.X)
        {
            tar.Remove();
        }

        DrawChunkHandle();
    }

    public void DrawChunkHandle()
    {
        if(!(Event.current.modifiers == EventModifiers.Control))
        {
            Handles.color = Color.Lerp(Color.black, Color.gray, 0.5f); 
            Handles.CubeHandleCap(0, tar.CenterLocal, Quaternion.identity, ChunkCube.HandleSize, EventType.Repaint);

            DrawPotentialNeighbors();
        }

    }

    private void DrawPotentialNeighbors()
    {
        Vector3 coords = tar.CenterLocal;
        //x neighbors
        DrawPotentialNeighborsForAxis(coords, Vector3.right * ChunkCube.HandleSize);

        //y neighbors
        DrawPotentialNeighborsForAxis(coords,Vector3.up * ChunkCube.HandleSize);


        //z neighbors
        DrawPotentialNeighborsForAxis(coords,Vector3.forward * ChunkCube.HandleSize);

    }

    void DrawPotentialNeighborsForAxis(Vector3 coords, Vector3 dir)
    {
        Vector3 pos = coords + dir;
        if (!tar.Manager.ContainsChunkByCoord(tar.MakeLocalToWorldChunkCoord(pos)))
        {
            DrawPotentialHandle(pos);
        }

        pos = coords - dir;
        if (!tar.Manager.ContainsChunkByCoord(tar.MakeLocalToWorldChunkCoord(pos)))
        {
            DrawPotentialHandle(pos);
        }
    }

    void DrawPotentialHandle(Vector3 pos)
    {
        Handles.color = Color.Lerp(Color.yellow, Color.white, .25f);
        if (Handles.Button(pos, Quaternion.identity, ChunkCube.PotentialSize, ChunkCube.PotentialSize, Handles.CubeHandleCap))
        {
            pos = tar.MakeLocalToWorldChunkCoord(pos);
            tar.Manager.AddChunk(pos);
        }
    }
}

