using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

public class SurfaceGenerator : MonoBehaviour
{
    public event Action<Chunk> OnChunkGenreated;
    [SerializeField][Range(1,100)] int resolution = 10;
    //[SerializeField][Range(0.1f,10f)] float sphereRadius =2f;
    [SerializeField] Vector3 center = Vector3.zero;
    [SerializeField] Chunk testChunk = null;
    [SerializeField] MeshFilter mesh = null;
    [SerializeField] bool sphere = false;

    Transform terrain;
    public Transform Terrain
    {
        get
        {
            if (terrain == null)
                CreateTerrainParent();
            return terrain;
        }
        protected set
        {
            terrain = value;
        }
    }

    Queue<MeshDataForChunk> meshQueue = new Queue<MeshDataForChunk>();

    public bool IsUseableInEditor { get; protected set; }

    private void Awake()
    {
        if (Terrain == null)
        {
            CreateTerrainParent();
        }
    }

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

    public void RequestMesh(List<ChunkToGenerate> chunksToGen, Type idensityFunc, int resolution, bool flatShaded)
    {
        //TODO maybe instance for each and every one extra instead of one
        IDensityFunc densFunc = Activator.CreateInstance(idensityFunc) as IDensityFunc;

        for (int i = 0; i < chunksToGen.Count; i++)
        {
            var chunk = chunksToGen[i].Chunk;
            ThreadedDataRequester.Instance.RequestData(() => { return chunk.CubeMarch(resolution, densFunc,flatShaded); }, (obj) => OnDataRecieved((MeshData)obj ,chunk) );
        }

    }

    public void RequestMesh(List<ChunkToGenerate> chunksToGen, int resolution, bool flatShaded)
    {
        for (int i = 0; i < chunksToGen.Count; i++)
        {
            IDensityFunc f = Activator.CreateInstance(chunksToGen[i].DensityFunc) as IDensityFunc;
            var chunk = chunksToGen[i].Chunk;

            ThreadedDataRequester.Instance.RequestData(() => { return chunk.CubeMarch(resolution, f, flatShaded); }, (obj) => OnDataRecieved((MeshData)obj, chunk));
        }
    }

    void OnDataRecieved(object obj)
    {
        MeshData data = (MeshData)obj;


        for (int a = 0; a < data.triangles.Length; a++)
        {
            if (data.triangles[a] >= data.vertices.Length)
            {
                Debug.Log($"{a}: referencing out of bounds vert: {data.triangles[a]} ");
            }
        }

        data.ToMesh(mesh.sharedMesh);
        OnChunkGenreated?.Invoke(testChunk);
    }

    public void OnDataRecieved(MeshData data, Chunk chunk)
    {
        meshQueue.Enqueue(new MeshDataForChunk(chunk, data));
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    public static void OnRecomplie()
    {
        var surGen = FindObjectOfType<SurfaceGenerator>();
        if(surGen !=  null)
        {
            surGen.MakeUseableInEditor();
        }
    }

    void OnUpdate()
    {
        if (meshQueue.Count > 0)
        {
            var mData = meshQueue.Dequeue();
            BuildMeshForChunk(mData);
        }
    }

    public void MakeUseableInEditor()
    {
        EditorApplication.update -= OnUpdate;
        EditorApplication.update += OnUpdate;
        EditorApplication.quitting -= OnQuit;
        EditorApplication.quitting += OnQuit;
        IsUseableInEditor = true;
    }

    private void OnQuit()
    {
        EditorApplication.update -= OnUpdate;
        IsUseableInEditor = false;
    }

    private void OnDestroy()
    {
        IsUseableInEditor = false;//unnecessary lol
        EditorApplication.update -= OnUpdate;
    }

    void BuildMeshForChunk(MeshDataForChunk mData)
    {
        if (Terrain == null)
        {
            CreateTerrainParent();
        }

        //Create A gameobject with mesh filter and renderer
        GameObject newChunk = new GameObject();
        newChunk.transform.position = mData.Chunk.Center;
        newChunk.transform.rotation = Quaternion.identity;
        newChunk.transform.SetParent(Terrain);
        newChunk.name = $"Chunk: {mData.Chunk.Center}";

        var mFilter = newChunk.AddComponent<MeshFilter>();

        var mRenderer = newChunk.AddComponent<MeshRenderer>();
        mRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

        mFilter.sharedMesh = mData.Data.ToMesh();
        OnChunkGenreated?.Invoke(mData.Chunk);
    }

    private void CreateTerrainParent()
    {
        var root = GameObject.FindGameObjectWithTag("TerrainRoot");
        if (root == null)
        {
            GameObject tObj = new GameObject();
            tObj.name = "Terrain";
            tObj.tag = "TerrainRoot";
            tObj.transform.position = Vector3.zero;
            tObj.transform.rotation = Quaternion.identity;
            Terrain = tObj.transform;
        }
        else
        {
            Terrain = root.transform;
        }
    }

    private void OnDrawGizmos()
    {
        if(testChunk != null)
        {
            testChunk.Visualize();
        }
    }

    struct MeshDataForChunk
    {
        public Chunk Chunk;
        public MeshData Data;

        public MeshDataForChunk(Chunk chunk, MeshData data)
        {
            Chunk = chunk;
            Data = data;
        }
    }

}