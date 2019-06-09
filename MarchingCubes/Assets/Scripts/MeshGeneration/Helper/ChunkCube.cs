using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkCube : MonoBehaviour
{
    public static float HandleSize = 2f;
    public static float PotentialSize { get { return HandleSize * .25f; } }
    Chunk c;
    public Chunk ChunkToHandle {
        get { return c; }
        set
        {
            c = value;
            transform.position = CenterLocal;
        }
    }
    public ChunkManager Manager;

    public Vector3 CenterLocal
    {
        get
        {
            Vector3 chunkNormViaExtends = ChunkToHandle.Center;
            Vector3 ext = ChunkToHandle.Extents;

            chunkNormViaExtends.Scale(new Vector3(
                1.0f / (ext.x),
                1.0f / (ext.y),
                1.0f / (ext.z)
                ));

            return chunkNormViaExtends * HandleSize;
        }
    }

    public void Remove()
    {
        Manager.Remove(this);
    }

    public Vector3 Extents
    {
        get
        {
            return ChunkToHandle.Extents;
        }
    }

    public Vector3 Center
    {
        get
        {
            return ChunkToHandle.Center;
        }
    }

    public Vector3 MakeLocalToWorldChunkCoord(Vector3 coord)
    {
        //add new chunk
        coord.Scale(new Vector3(1.0f / HandleSize, 1.0f / HandleSize, 1.0f / HandleSize));
        coord.Scale(Chunk.DefaultExtents);
        return coord;
    }
}
