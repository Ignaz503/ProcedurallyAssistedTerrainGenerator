using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public ChunksToManage ToManage;

    public bool ContainsChunkByCoord(Vector3 coord)
    {
        for (int i = 0; i < ToManage.Chunks.Count; i++)
        {
            var chunk = ToManage.Chunks[i];

            if ((chunk.Chunk.Center - coord).sqrMagnitude <= 0f)
            {
                return true;
            }
        }
        return false;
    }

    public void AddChunk(Vector3 pos)
    {
        ToManage.Chunks.Add(new Generate(new Chunk(pos, Chunk.DefaultExtents)));
    }

    public void RemoveChunk(Chunk chunk)
    {
        if (ToManage.Chunks.Count > 1)
        {
            ToManage.RemoveChunk(chunk);
        }
        else
        {
            Debug.LogWarning("One chunk must remain (Only one will reign supreme)");
        }
    }
}

public struct ChunksToManage
{
    public List<Generate> Chunks;

    public void RemoveChunk(Chunk chunk)
    {
        for (int i = Chunks.Count -1 ; i >= 0; i--)
        {
            if (Chunks[i].Chunk == chunk)
            {
                Chunks[i] = Chunks[Chunks.Count - 1];//switch last elem to this
                Chunks.RemoveAt(Chunks.Count - 1); //remove last elem
                return;
            }
        }
    }

    public void AddChunk(Vector3 pos)
    {
        Chunks.Add(new Generate(new Chunk(pos, Chunk.DefaultExtents)));
    }

    public void RemoveChunk(Generate chunkInfo)
    {
        Chunks.Remove(chunkInfo);
    }

    public void RemoveChunkAt(int i, bool keepOrder = false)
    {
        if (!keepOrder)
        {
            Chunks[i] = Chunks[Chunks.Count - 1];
            Chunks.RemoveAt(Chunks.Count - 1);
        }
        else
        {
            Chunks.RemoveAt(i);
        }
    }

    public void UpdateExtents(Vector3 newExt)
    {
        for (int i = 0; i < Chunks.Count; i++)
        {
            Chunks[i].Chunk.Extents = newExt;
        }
    }
}

[Serializable]
public class Generate
{
    public Chunk Chunk;
    public Type DensityFunc;

    public Generate(Chunk chunk, Type densityFunc)
    {
        this.Chunk = chunk ?? throw new ArgumentNullException(nameof(chunk));
        DensityFunc = densityFunc ?? throw new ArgumentNullException(nameof(densityFunc));
    }

    public Generate(Chunk chunk)
    {
        this.Chunk = chunk ?? throw new ArgumentNullException(nameof(chunk));
    }

    public void SetDensityFunction(Type t)
    {
        DensityFunc = t;
    }
}
