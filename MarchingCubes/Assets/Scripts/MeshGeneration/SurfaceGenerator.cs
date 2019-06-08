using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SurfaceGenerator : MonoBehaviour
{
    [SerializeField][Range(1,100)] int resolution = 10;
    //[SerializeField][Range(0.1f,10f)] float sphereRadius =2f;
    [SerializeField] Vector3 center = Vector3.zero;
    [SerializeField] Chunk testChunk = null;
    [SerializeField] MeshFilter mesh = null;
    [SerializeField] bool sphere = false;

    public Transform Terrain { get; protected set; }

    public void Generate()
    {
        if(mesh != null)
        {
            RequestMesh();
        }
        else
        {
            Debug.Log("Need meshfilter to assign generated mesh");
        }
    }

    void RequestMesh()
    {
        IDensityFunc func;
        if (sphere)
            func = new SphereDensityFunc(2f, center);
        else
            func = new SimpleSurface();
        ThreadedDataRequester.Instance.RequestData(()=> { return testChunk.CubeMarch(resolution, func); }, OnDataRecieved);
    }


    public void RequestMesh(List<Generate> chunksToGen, Type idensityFunc, int resolution)
    {
        //TODO maybe instance for each and every one extra instead of one
        IDensityFunc densFunc = Activator.CreateInstance(idensityFunc) as IDensityFunc;

        for (int i = 0; i < chunksToGen.Count; i++)
        {
            var chunk = chunksToGen[i].Chunk;
            ThreadedDataRequester.Instance.RequestData(() => { return chunk.CubeMarch(resolution, densFunc); }, (obj) => OnDataRecieved((MeshData)obj ,chunk) );
        }

    }

    public void RequestMesh(List<Generate> chunksToGen, int resolution)
    {
        for (int i = 0; i < chunksToGen.Count; i++)
        {
            IDensityFunc f = Activator.CreateInstance(chunksToGen[i].DensityFunc) as IDensityFunc;
            var chunk = chunksToGen[i].Chunk;

            ThreadedDataRequester.Instance.RequestData(() => { return chunk.CubeMarch(resolution, f); }, (obj) => OnDataRecieved((MeshData)obj, chunk));
        }
    }

    void OnDataRecieved(object obj)
    {
        MeshData data = (MeshData)obj;
        data.ToMesh(mesh.sharedMesh);
        Debug.Log("Added Mesh");
    }

    public void OnDataRecieved(MeshData data, Chunk chunk)
    {
        if (Terrain == null)
        {
            CreateTerrainParent();
        }

        //Create A gameobject with mesh filter and renderer
        GameObject newChunk = new GameObject();
        newChunk.transform.position = chunk.Center;
        newChunk.transform.rotation = Quaternion.identity;
        newChunk.transform.SetParent(Terrain);
        newChunk.name = $"Chunk: {chunk.Center}";

        var mFilter = newChunk.AddComponent<MeshFilter>();

        var mRenderer = newChunk.AddComponent<MeshRenderer>();
        mRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

        mFilter.sharedMesh = data.ToMesh();
    }

    private void CreateTerrainParent()
    {
        GameObject tObj = new GameObject();
        tObj.name = "Terrain";
        tObj.tag = "TerrainRoot";
        tObj.transform.position = Vector3.zero;
        tObj.transform.rotation = Quaternion.identity;
        Terrain = tObj.transform;
    }

    private void OnDrawGizmos()
    {
        if(testChunk != null)
        {
            testChunk.Visualize();
        }
    }
}

public class SphereDensityFunc : IDensityFunc
{
    float rad;
    Vector3 center;

    public SphereDensityFunc(float rad, Vector3 center)
    {
        this.rad = rad;
        this.center = center;
    }

    public float Evaluate(SamplePointVariables x, SamplePointVariables y, SamplePointVariables z)
    {
        return Evaluate(new Vector3(x.ValueWorld, y.ValueWorld, z.ValueWorld));
    }

    public float Evaluate(Vector3 point)
    {
        return -(((point - center).magnitude - rad));//+ (Mathf.PerlinNoise(point.x,point.y));
    }
}

