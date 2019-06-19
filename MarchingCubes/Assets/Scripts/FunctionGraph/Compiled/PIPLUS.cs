using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PIPLUS : IDensityFunc
{
	public float Evaluate(SamplePointVariables x, SamplePointVariables y, SamplePointVariables z)
	{
		return ( Mathf.PI + Mathf.PI ) ;
	}
}
