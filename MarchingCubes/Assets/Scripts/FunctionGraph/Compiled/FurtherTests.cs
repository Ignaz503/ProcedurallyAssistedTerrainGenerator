using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurtherTests : IDensityFunc
{
	public float Evaluate(SamplePointVariables x, SamplePointVariables y, SamplePointVariables z)
	{
		return ( ( -y.ValueWorld ) + SimplexNoise.Evaulate( x.ValueWorld , y.ValueWorld , z.ValueWorld ) ) ;
	}

    public float Evaluate(Vector3 point)
    {
        return -1;//+ (Mathf.PerlinNoise(point.x,point.y));
    }
}
