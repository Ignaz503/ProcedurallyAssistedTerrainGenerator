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
}

public struct ChunksToManage
{
    public List<ChunkToGenerate> Chunks;

}

[Serializable]
public struct ChunkToGenerate
{
    public Chunk chunk;
    public Type DensityFunc;

    public ChunkToGenerate(Chunk chunk, Type densityFunc)
    {
        this.chunk = chunk ?? throw new ArgumentNullException(nameof(chunk));
        DensityFunc = densityFunc ?? throw new ArgumentNullException(nameof(densityFunc));
    }

    public ChunkToGenerate(Chunk chunk) : this()
    {
        this.chunk = chunk ?? throw new ArgumentNullException(nameof(chunk));
    }

    public void SetDensityFunction(Type t)
    {
        DensityFunc = t;
    }
}
