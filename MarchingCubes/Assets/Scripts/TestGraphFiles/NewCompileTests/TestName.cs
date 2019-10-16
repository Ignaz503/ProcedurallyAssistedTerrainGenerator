namespace FuncGraphCompiled._TestName
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	 
	class TestName : IDensityFunc
	{
		public float Evaluate(SamplePointVariables x, SamplePointVariables y, SamplePointVariables z)
		{
			return  SimplexNoise.Evaulate( x.ValueWorld , 0f , 0f ) ;
		}

		public  TestName()
		{
		}

	}
}
