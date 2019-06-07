using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGraph : IDensityFunc
{
	public float Evaluate(SamplePointVariables x, SamplePointVariables y, SamplePointVariables z)
	{
		return ( ( -y.ValueWorld ) + Mathf.PerlinNoise( x.ValueWorld , z.ValueWorld ) ) ;
	}

    public float Evaluate(Vector3 point)
    {
        return -1;//+ (Mathf.PerlinNoise(point.x,point.y));
    }

}
