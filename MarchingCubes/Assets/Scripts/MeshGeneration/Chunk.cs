using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Chunk 
{
    public static Vector3 DefaultExtents;

    [SerializeField] Bounds chunkBounds;

#if UNITY_EDITOR
    [SerializeField] bool visualizeBounds = false;
#endif

    public Vector3 Center
    {
        get
        {
            return chunkBounds.center;
        }
        set
        {
            chunkBounds.center = value;
        }
    }

    public Vector3 Extents
    {
        get { return chunkBounds.extents; }
        set
        {
            Vector3 oldExt = chunkBounds.extents;
            chunkBounds.extents = value;

            Vector3 center = Center;
            center.Scale(new Vector3(chunkBounds.extents.x / oldExt.x, chunkBounds.extents.y / oldExt.y, chunkBounds.extents.z / oldExt.z));
            Center = center;
        }
    }

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

    Vector3 WorldToLocal(Vector3 world)
    {
        return world - Center;
    }

    public MeshData CubeMarch(float girdResolution, IDensityFunc func, float isoLevel = 0f)
    {
        return CubeMarch(Vector3.one * girdResolution, func, isoLevel);
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
                                VertexA_ID = new PointCreators(voxel[idxA0].ID, voxel[idxB0].ID),
                                B = CubeMarchLerp(voxel[idxA1], voxel[idxB1], isoLevel),
                                VertexB_ID = new PointCreators(voxel[idxA1].ID, voxel[idxB1].ID),
                                C = CubeMarchLerp(voxel[idxA2], voxel[idxB2], isoLevel),   
                               VertexC_ID = new PointCreators(voxel[idxA2].ID, voxel[idxB2].ID) 
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
                vertices[triIdx * 3 + vertIdx] = WorldToLocal(trianglesFromMarch[triIdx][vertIdx]);
                triangles[triIdx * 3 + vertIdx] = triIdx * 3 + vertIdx;
            }
        }

        MeshData m = new MeshData()
        {
            vertices = vertices,
            triangles = triangles
        };

        //List<Vector3> vert = new List<Vector3>();
        //List<Vector2> uvs = new List<Vector2>();
        //int[] tri = new int[trianglesFromMarch.Count * 3];

        //for (int triIdx = 0; triIdx < trianglesFromMarch.Count; triIdx++)
        //{
        //    for (int vertIdx = 0; vertIdx < 3; vertIdx++)
        //    {
        //        int actualTriIDX = triIdx * 3 + vertIdx;

        //        Vector3 vertToAdd = WorldToLocal(trianglesFromMarch[triIdx][vertIdx]);
        //        int vertID = vert.Count - 1;
        //        bool foundOverlaping = false;

        //        for (int i = 0; i < vert.Count; i++)
        //        {
        //            if ((vertToAdd - vert[i]).sqrMagnitude <= 0)
        //            {
        //                vertID = i;
        //                foundOverlaping = true;
        //                break;
        //            }
        //        }
        //        if (!foundOverlaping)
        //        {
        //            vert.Add(vertToAdd);
        //            uvs.Add(Vector2.zero);
        //            vertID = vert.Count - 1;
        //        }
        //        tri[actualTriIDX] = vertID;
        //    }
        //}

        //MeshData m = new MeshData()
        //{
        //    vertices = vert.ToArray(),
        //    uvs = uvs.ToArray(),
        //    triangles = tri
        //};

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

        Vector3 localP = GetLocalPoint(x, y, z + 1, gridResolution);
        Vector3 p = LocalToWorld(localP);

        voxel[0] = new Vertex
        {
            ID = new VertexID(x,  y,  z + 1),
            Point = p,
            IsoVal = densFunc.Evaluate(
            new SamplePointVariables(p.x, localP.x),
            new SamplePointVariables(p.y, localP.y),
            new SamplePointVariables(p.z, localP.z)
            )
        };

        localP = GetLocalPoint(x + 1, y, z + 1, gridResolution);
        p = LocalToWorld(localP);

        voxel[1] = new Vertex
        {
            ID = new VertexID(x + 1,  y,  z + 1),
            Point = p,
            IsoVal = densFunc.Evaluate(
            new SamplePointVariables(p.x, localP.x),
            new SamplePointVariables(p.y, localP.y),
            new SamplePointVariables(p.z, localP.z)
            )
        };

        localP = GetLocalPoint(x + 1, y, z, gridResolution);
        p = LocalToWorld(localP);
        voxel[2] = new Vertex
        {
            ID = new VertexID(x + 1,  y,  z),
            Point = p,
            IsoVal = densFunc.Evaluate(
            new SamplePointVariables(p.x, localP.x),
            new SamplePointVariables(p.y, localP.y),
            new SamplePointVariables(p.z, localP.z)
            )
        };

        localP = GetLocalPoint(x, y, z, gridResolution);
        p = LocalToWorld(localP);
        voxel[3] = new Vertex
        {
            ID = new VertexID(x, y,  z),
            Point = p,
            IsoVal = densFunc.Evaluate(
            new SamplePointVariables(p.x, localP.x),
            new SamplePointVariables(p.y, localP.y),
            new SamplePointVariables(p.z, localP.z)
            )
        };

        localP = GetLocalPoint(x, y + 1, z + 1, gridResolution);
        p = LocalToWorld(localP);
        voxel[4] = new Vertex
        {
            ID = new VertexID(x, y + 1,  z + 1),
            Point = p,
            IsoVal = densFunc.Evaluate(
            new SamplePointVariables(p.x, localP.x),
            new SamplePointVariables(p.y, localP.y),
            new SamplePointVariables(p.z, localP.z)
            )
        };

        localP = GetLocalPoint(x + 1, y + 1, z + 1, gridResolution);
        p = LocalToWorld(localP);
        voxel[5] = new Vertex
        {
            ID = new VertexID(x + 1, y + 1,  z + 1),
            Point = p,
            IsoVal = densFunc.Evaluate(
            new SamplePointVariables(p.x, localP.x),
            new SamplePointVariables(p.y, localP.y),
            new SamplePointVariables(p.z, localP.z)
            )
        };

        localP = GetLocalPoint(x + 1, y + 1, z, gridResolution);
        p = LocalToWorld(localP);
        voxel[6] = new Vertex
        {
            ID = new VertexID(x + 1, y + 1,  z),
            Point = p,
            IsoVal = densFunc.Evaluate(
            new SamplePointVariables(p.x, localP.x),
            new SamplePointVariables(p.y, localP.y),
            new SamplePointVariables(p.z, localP.z)
            )
        };

        localP = GetLocalPoint(x, y + 1, z, gridResolution);
        p = LocalToWorld(localP);
        voxel[7] = new Vertex
        {
            ID = new VertexID( x,  y + 1,  z ),
            Point = p,
            IsoVal = densFunc.Evaluate(
            new SamplePointVariables(p.x, localP.x),
            new SamplePointVariables(p.y, localP.y),
            new SamplePointVariables(p.z, localP.z)
            )
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
        public VertexID ID;
        public Vector3 Point;
        public float IsoVal;

    }

    struct VertexID
    {
        int X;
        int Y;
        int Z;

        public VertexID(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override int GetHashCode()
        {
            var hashCode = -307843816;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Z.GetHashCode();
            return hashCode;
        }
    }

    struct PointCreators
    {
        VertexID First;
        VertexID Second;

        public PointCreators(VertexID first, VertexID second)
        {
            First = first;
            Second = second;
        }


        public override int GetHashCode()
        {
            var hashCode = 43270662;
            hashCode = hashCode * -1521134295 + First.GetHashCode();
            hashCode = hashCode * -1521134295 + Second.GetHashCode();
            return hashCode;
        }
    }

    struct Triangle
    {
        public Vector3 A;
        public PointCreators VertexA_ID;
        public Vector3 B;
        public PointCreators VertexB_ID;
        public Vector3 C;
        public PointCreators VertexC_ID;

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

        public PointCreators GetPointCreator(int idx)
        {
            switch (idx)
            {
                case 0:
                    return VertexA_ID;
                case 1:
                    return VertexB_ID;
                case 2:
                    return VertexB_ID;
                default:
                    throw new System.IndexOutOfRangeException($"Only three vertices per triangle index must be between [0..2] and not {idx}");
            }
        }

    }

}



public struct MeshData
{
    public Vector3[] vertices;
    public Vector2[] uvs;
    public int[] triangles;


    public Mesh ToMesh(Mesh m)
    {
        m.Clear();
        m.vertices = vertices;
        m.triangles = triangles;
        m.uv = uvs;
        m.RecalculateNormals();
        m.RecalculateTangents();
        return m;
    }

    public Mesh ToMesh()
    {
        Mesh m = new Mesh();
        m.vertices = vertices;
        m.triangles = triangles;
        m.uv = uvs;
        m.RecalculateNormals();
        m.RecalculateNormals();
        m.RecalculateTangents();
        return m;
    }

}