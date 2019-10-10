namespace FuncGraphCompiled._FirstRecurssionTest
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	 
	class FirstRecurssionTest : IDensityFunc
	{
		public float Evaluate(SamplePointVariables x, SamplePointVariables y, SamplePointVariables z)
		{
			return  Mathf.PerlinNoise( x.ValueWorld , y.ValueWorld ) ;
		}

		public  FirstRecurssionTest()
		{
		}

	}
}
