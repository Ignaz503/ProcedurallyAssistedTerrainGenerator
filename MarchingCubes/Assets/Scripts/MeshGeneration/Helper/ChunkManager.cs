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

            if ((chunk.chunk.Center - coord).sqrMagnitude <= 0f)
            {
                return true;
            }
        }
        return false;
    }

    public void AddChunk(Vector3 pos)
    {
        ToManage.Chunks.Add(new ChunkToGenerate(new Chunk(pos, Chunk.DefaultExtents)));
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
    public List<ChunkToGenerate> Chunks;

    public void RemoveChunk(Chunk chunk)
    {
        for (int i = Chunks.Count -1 ; i >= 0; i--)
        {
            if (Chunks[i].chunk == chunk)
            {
                Chunks[i] = Chunks[Chunks.Count - 1];//switch last elem to this
                Chunks.RemoveAt(Chunks.Count - 1); //remove last elem
                return;
            }
        }
    }

    public void AddChunk(Vector3 pos)
    {
        Chunks.Add(new ChunkToGenerate(new Chunk(pos, Chunk.DefaultExtents)));
    }

    public void RemoveChunk(ChunkToGenerate chunkInfo)
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
}

[Serializable]
public class ChunkToGenerate
{
    public Chunk chunk;
    public Type DensityFunc;

    public ChunkToGenerate(Chunk chunk, Type densityFunc)
    {
        this.chunk = chunk ?? throw new ArgumentNullException(nameof(chunk));
        DensityFunc = densityFunc ?? throw new ArgumentNullException(nameof(densityFunc));
    }

    public ChunkToGenerate(Chunk chunk)
    {
        this.chunk = chunk ?? throw new ArgumentNullException(nameof(chunk));
    }

    public void SetDensityFunction(Type t)
    {
        DensityFunc = t;
    }
}
