using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecursionTest : IDensityFunc
{
	public float Evaluate(SamplePointVariables x, SamplePointVariables y, SamplePointVariables z)
	{
		return ( Mathf.Ceil( Mathf.PI ) + Mathf.Floor( Mathf.PI ) ) ;
	}
}
