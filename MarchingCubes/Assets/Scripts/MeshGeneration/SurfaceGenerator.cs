using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceGenerator : MonoBehaviour
{
    [SerializeField][Range(1,100)] int resolution = 10;
    //[SerializeField][Range(0.1f,10f)] float sphereRadius =2f;
    [SerializeField] Vector3 center = Vector3.zero;
    [SerializeField] Chunk testChunk = null;
    [SerializeField] MeshFilter mesh = null;


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
        ThreadedDataRequester.Instance.RequestData(()=> { return testChunk.CubeMarch(resolution, new SimpleSurface()); }, OnDataRecieved);
    }

    void OnDataRecieved(object obj)
    {
        MeshData data = (MeshData)obj;
        data.ToMesh(mesh.sharedMesh);
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

    protected float Evaluate(Vector3 point)
    {
        return -(((point - center).magnitude - rad));//+ (Mathf.PerlinNoise(point.x,point.y));
    }
}

