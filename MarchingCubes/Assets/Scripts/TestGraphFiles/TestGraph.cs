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
}
