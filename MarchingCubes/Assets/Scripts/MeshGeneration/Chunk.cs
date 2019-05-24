using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Chunk 
{
    [SerializeField] Bounds chunkBounds;

#if UNITY_EDITOR
    [SerializeField] bool visualizeBounds = false;
#endif

    public Chunk(Bounds chunkBounds)
    {
        this.chunkBounds = chunkBounds;
    }

    public Chunk(Vector3 centerPoint, float extents)
    {
        chunkBounds = new Bounds()
        {
            center = centerPoint,
            extents = Vector3.one * extents
        };
    }

    public Chunk()
    {}

    public Chunk(Vector3 centerPoint, Vector3 boundExtents)
    {
        chunkBounds = new Bounds()
        {
            center = centerPoint,
            extents = boundExtents
        };
    }

    /// <summary>
    /// assumed to be in -0.5 to 0.5
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    Vector3 LocalToWorld(Vector3 v)
    {
        Vector3 ex = chunkBounds.extents;
        ex.Scale(v);
        return chunkBounds.center + ex;
    }

    public MeshData CubeMarch(float girdResolution, IDensityFunc func, float isoLevel = 0f)
    {
        return CubeMarch(Vector3.one * girdResolution, func, isoLevel);
    }

    float x;
    public float GetX()
    {
        return x;
    }

    public MeshData CubeMarch(Vector3 gridResolution, IDensityFunc func, float isoLevel = 0f)
    {

        List<Triangle> trianglesFromMarch = new List<Triangle>();
        //loop over everything
        //stop at  < gridResolution as we need to calculate the positions +1 anyways and the alst rows colums fileds voxel is out of the chunkbounds anyway
        for (int z = 0; z < gridResolution.z; z++)
        {
            for (int y = 0; y < gridResolution.y; y++)
            {
                for (int x = 0; x < gridResolution.x; x++)
                {
                    Vertex[] voxel = GetVoxel(x, y, z, gridResolution, func);
                    int lookUpIdx = GetLookUpIndex(voxel,isoLevel);

                    for (int i = 0; MeshGenerationLUTs.triTable[lookUpIdx][i] != -1; i+=3)
                    {
                        int idxA0 = MeshGenerationLUTs.cornerIndexAFromEdge[MeshGenerationLUTs.triTable[lookUpIdx][i]];
                        int idxB0 = MeshGenerationLUTs.cornerIndexBFromEdge[MeshGenerationLUTs.triTable[lookUpIdx][i]];

                        int idxA1 = MeshGenerationLUTs.cornerIndexAFromEdge[MeshGenerationLUTs.triTable[lookUpIdx][i+1]];
                        int idxB1 = MeshGenerationLUTs.cornerIndexBFromEdge[MeshGenerationLUTs.triTable[lookUpIdx][i+1]];

                        int idxA2 = MeshGenerationLUTs.cornerIndexAFromEdge[MeshGenerationLUTs.triTable[lookUpIdx][i+2]];
                        int idxB2 = MeshGenerationLUTs.cornerIndexBFromEdge[MeshGenerationLUTs.triTable[lookUpIdx][i+2]];


                        trianglesFromMarch.Add(
                            new Triangle()
                            {
                                A = CubeMarchLerp(voxel[idxA0], voxel[idxB0], isoLevel),
                                B = CubeMarchLerp(voxel[idxA1], voxel[idxB1], isoLevel),
                                C = CubeMarchLerp(voxel[idxA2], voxel[idxB2], isoLevel)   
                            }
                            );

                    }
                }
            }
       }// end for z

        Vector3[] vertices = new Vector3[trianglesFromMarch.Count * 3];
        int[] triangles = new int[trianglesFromMarch.Count * 3];

        for (int triIdx = 0; triIdx < trianglesFromMarch.Count; triIdx++)
        {
            for (int vertIdx = 0; vertIdx < 3; vertIdx++)
            {
                vertices[triIdx * 3 + vertIdx] = trianglesFromMarch[triIdx][vertIdx];
                triangles[triIdx * 3 + vertIdx] = triIdx * 3 + vertIdx;
            }
        }
        MeshData m = new MeshData()
        {
            vertices = vertices,
            triangles = triangles
        };
        return m;
    }

    Vector3 CubeMarchLerp(Vertex a, Vertex b, float isoLevel)
    {
        if (CubeMarchComparisonLessThan(b, a))
        {
            Vertex temp = a;
            a = b;
            b = temp;
        }

        if (Mathf.Abs(a.IsoVal - b.IsoVal) > 0.00001)
        {
            return a.Point + (b.Point - a.Point) / (b.IsoVal - a.IsoVal)*(isoLevel - a.IsoVal);
        }
        else
            return a.Point;

    }

    bool CubeMarchComparisonLessThan(Vertex a, Vertex b)
    {
        if (a.Point.x < b.Point.x)
            return true;
        else if(a.Point.x > b.Point.x)
            return false;

        if (a.Point.y < b.Point.y)
            return true;
        else if (a.Point.y > b.Point.y)
            return false;

        if (a.Point.z < b.Point.z)
            return true;
        else if (a.Point.z > b.Point.z)
            return false;

        return false;
    }


    int GetLookUpIndex(Vertex[] voxel, float isoLevel)
    {
        int idx = 0;

        if (voxel[0].IsoVal < isoLevel)
            idx |= 1;
        if (voxel[1].IsoVal < isoLevel)
            idx |= 2;
        if (voxel[2].IsoVal < isoLevel)
            idx |= 4;
        if (voxel[3].IsoVal < isoLevel)
            idx |= 8;
        if (voxel[4].IsoVal < isoLevel)
            idx |= 16;
        if (voxel[5].IsoVal < isoLevel)
            idx |= 32;
        if (voxel[6].IsoVal < isoLevel)
            idx |= 64;
        if (voxel[7].IsoVal < isoLevel)
            idx |= 128;

        return idx;
    }


    /// <summary>
    /// Transform point into range -05 to 0.5
    /// based on total resolution of chunk
    /// </summary>
    Vector3 GetLocalPoint(float x, float y, float z, Vector3 resolution)
    {
        return new Vector3(x / resolution.x, y / resolution.y, z / resolution.z) - Vector3.one * .5f;
    }

    Vertex[] GetVoxel(int x, int y, int z, Vector3 gridResolution, IDensityFunc densFunc)
    {
        Vertex[] voxel = new Vertex[8];

        Vector3 p = LocalToWorld(GetLocalPoint(x, y, z + 1, gridResolution));

        voxel[0] = new Vertex
        {
            Point = p,
            IsoVal = densFunc.Evaluate(p)
        };

        p = LocalToWorld(GetLocalPoint(x + 1, y, z + 1, gridResolution));
        voxel[1] = new Vertex
        {
            Point = p,
            IsoVal = densFunc.Evaluate(p)
        };

        p = LocalToWorld(GetLocalPoint(x + 1, y, z, gridResolution));
        voxel[2] = new Vertex
        {
            Point = p,
            IsoVal = densFunc.Evaluate(p)
        };

        p = LocalToWorld(GetLocalPoint(x, y, z, gridResolution));
        voxel[3] = new Vertex
        {
            Point = p,
            IsoVal = densFunc.Evaluate(p)
        };

        p = LocalToWorld(GetLocalPoint(x, y + 1, z + 1, gridResolution));
        voxel[4] = new Vertex
        {
            Point = p,
            IsoVal = densFunc.Evaluate(p)
        };

        p = LocalToWorld(GetLocalPoint(x + 1, y + 1, z + 1, gridResolution));
        voxel[5] = new Vertex
        {
            Point = p,
            IsoVal = densFunc.Evaluate(p)
        };

        p = LocalToWorld(GetLocalPoint(x + 1, y + 1, z, gridResolution));
        voxel[6] = new Vertex
        {
            Point = p,
            IsoVal = densFunc.Evaluate(p)
        };

        p = LocalToWorld(GetLocalPoint(x, y + 1, z, gridResolution));
        voxel[7] = new Vertex
        {
            Point = p,
            IsoVal = densFunc.Evaluate(p)
        };

        return voxel;
    }

#if UNITY_EDITOR
    public void Visualize()
    {
        if (visualizeBounds)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireCube(chunkBounds.center, chunkBounds.extents);
        }
    }
#endif

    struct Vertex
    {
        public Vector3 Point;
        public float IsoVal;
    }

    struct Triangle
    {
        public Vector3 A;
        public Vector3 B;
        public Vector3 C;

        public Vector3 this[int idx]
        {
            get
            {
                switch(idx)
                {
                    case 0:
                        return A;
                    case 1:
                        return B;
                    case 2:
                        return C;
                    default:
                        throw new System.IndexOutOfRangeException($"Only three vertices per triangle index must be between [0..2] and not {idx}");
                }
            }
        }
    }

}



public struct MeshData
{
    public Vector3[] vertices;
    public int[] triangles;


    public Mesh ToMesh(Mesh m)
    {
        m.Clear();

        m.vertices = vertices;
        m.triangles = triangles;
        m.RecalculateNormals();
        return m;
    }

    public Mesh ToMesh()
    {
        Mesh m = new Mesh();
        m.vertices = vertices;
        m.triangles = triangles;
        m.RecalculateNormals();
        return m;
    }

}