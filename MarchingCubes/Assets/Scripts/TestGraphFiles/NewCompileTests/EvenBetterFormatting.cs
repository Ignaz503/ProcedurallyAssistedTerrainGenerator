namespace FuncGraphCompiled._EvenBetterFormatting
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	 
	class EvenBetterFormatting : IDensityFunc
	{
		public float Evaluate(SamplePointVariables x, SamplePointVariables y, SamplePointVariables z)
		{
			return  Mathf.PI;
		}

		public  EvenBetterFormatting()
		{
		}

	}
}
