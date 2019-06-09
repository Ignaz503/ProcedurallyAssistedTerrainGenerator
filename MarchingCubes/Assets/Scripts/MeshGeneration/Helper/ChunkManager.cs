using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ChunkManager : MonoBehaviour
{
    public ChunksToManage ToManage;

    CubePool pool = new CubePool();

    public void CreateChunks()
    {
        for (int i = 0; i < ToManage.Chunks.Count; i++)
        {
            var chunkToGen = ToManage.Chunks[i];
            CreateChunkCube(chunkToGen.Chunk);
        }
        Selection.activeObject = this;
    }

    void CreateChunkCube(Chunk c)
    {
        var cC = pool.Get();
        cC.ChunkToHandle = c;
        cC.Manager = this;
        Selection.activeObject = cC;
    }

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

    public void Remove(ChunkCube chunkCube)
    {
        if (RemoveChunk(chunkCube.ChunkToHandle))
        {
            pool.Return(chunkCube);
        }
    }

    public void AddChunk(Vector3 pos)
    {
        ToManage.Chunks.Add(new ChunkToGenerate(new Chunk(pos, Chunk.DefaultExtents)));
        CreateChunkCube(ToManage.Chunks[ToManage.Chunks.Count - 1].Chunk);
    }

    public bool RemoveChunk(Chunk chunk)
    {
        if (ToManage.Chunks.Count > 1)
        {
            ToManage.RemoveChunk(chunk);
            return true;
        }
        else
        {
            Debug.LogWarning("One chunk must remain (Only one will reign supreme)");
            return false;
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

    public void UpdateExtents(Vector3 newExt)
    {
        for (int i = 0; i < Chunks.Count; i++)
        {
            Chunks[i].Chunk.Extents = newExt;
        }
    }
}

[Serializable]
public class ChunkToGenerate
{
    public Chunk Chunk;
    public Type DensityFunc;

    public ChunkToGenerate(Chunk chunk, Type densityFunc)
    {
        this.Chunk = chunk ?? throw new ArgumentNullException(nameof(chunk));
        DensityFunc = densityFunc ?? throw new ArgumentNullException(nameof(densityFunc));
    }

    public ChunkToGenerate(Chunk chunk)
    {
        this.Chunk = chunk ?? throw new ArgumentNullException(nameof(chunk));
    }

    public void SetDensityFunction(Type t)
    {
        DensityFunc = t;
    }
}


public class CubePool
{
    Queue<ChunkCube> pool;

    public CubePool()
    {
        pool = new Queue<ChunkCube>();
    }

    public ChunkCube Get()
    {
        if (pool.Count > 0)
        {
            var obj = pool.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            var nC = GameObject.CreatePrimitive(PrimitiveType.Cube);
            return nC.AddComponent<ChunkCube>();
        }
    }

    public void Return(ChunkCube c)
    {
        c.gameObject.SetActive(false);
        pool.Enqueue(c);
    }
}