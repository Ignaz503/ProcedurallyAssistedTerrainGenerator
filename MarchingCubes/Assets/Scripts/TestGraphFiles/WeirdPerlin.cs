using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeirdPerlin : IDensityFunc
{
	public float Evaluate(SamplePointVariables x, SamplePointVariables y, SamplePointVariables z)
	{
		return Mathf.PerlinNoise( ( x.ValueWorld + y.ValueWorld ) , x.ValueWorld ) ;
	}
}
