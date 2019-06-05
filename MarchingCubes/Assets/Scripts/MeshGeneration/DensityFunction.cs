using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IDensityFunc
{
    float Evaluate(SamplePointVariables x, SamplePointVariables y, SamplePointVariables z);

}

public class SimpleSurface : IDensityFunc
{
    public float Evaluate(SamplePointVariables x, SamplePointVariables y, SamplePointVariables z)
    {
        return Evaluate(new Vector3(x.ValueWorld, y.ValueWorld, z.ValueWorld));
    }

    protected float Evaluate(Vector3 point)
    {
        return -point.y + Mathf.PerlinNoise(point.x, point.z) * 2f;
    }
}


//public class DensityFunction : IDensityFunc
//{
//    public float Evaluate(SamplePointVariables x, SamplePointVariables y, SamplePointVariables z)
//    {
        //Write Graph
//    }
//}