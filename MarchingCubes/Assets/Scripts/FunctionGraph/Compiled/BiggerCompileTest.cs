using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiggerCompileTest : IDensityFunc
{
	public float Evaluate(SamplePointVariables x, SamplePointVariables y, SamplePointVariables z)
	{
		return ( ( Mathf.Abs( x.ValueWorld ) + -4.6f ) * ( -y.ValueWorld ) ) ;
	}
}
