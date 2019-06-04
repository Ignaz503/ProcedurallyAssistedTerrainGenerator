using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompileTest : IDensityFunc
{
	public float Evaluate(SamplePointVariables x, SamplePointVariables y, SamplePointVariables z)
	{
		return ( 23.65f + Mathf.PI ) ;
	}
}
