using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public interface IDensityFunc
{
    float Evaluate(float x, float y, float z);
    float Evaluate(Vector3 point);
}

public class SimpleSurface : IDensityFunc
{
    public float Evaluate(float x, float y, float z)
    {
        return Evaluate(new Vector3(x, y, z));
    }

    public float Evaluate(Vector3 point)
    {
        return -point.y + Mathf.PerlinNoise(point.x, point.z) * 2f;
    }
}
