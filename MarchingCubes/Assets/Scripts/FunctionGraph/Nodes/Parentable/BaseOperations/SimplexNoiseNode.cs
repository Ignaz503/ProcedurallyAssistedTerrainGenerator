using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoiseTest;

public class SimplexNoiseNode : FixedSizeMultiChildNode
{
    OpenSimplexNoise noise;
    [SerializeField] string seed;

    public SimplexNoiseNode(FunctionGraph graph) : base(3, graph)
    {
        noise = new OpenSimplexNoise(seed.GetHashCode());
    }

    public override string ShortDescription { get { return "3D Simplex Noise"; } }

    public override float Evaluate()
    {
        return (float)noise.Evaluate(childNodes[0].Evaluate(), childNodes[1].Evaluate(), childNodes[2].Evaluate());
    }

    public override int ValidateSelf()
    {
        if (seed != null)
            return 0;
        else
        {
            FunctionGraphValidationMessageLogger.LogError("Simplex Noise Seed Can't be null");
            return 1;
        }
    }
}
