using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplexNoiseNode : FixedSizeMultiChildNode
{
    public SimplexNoiseNode(FunctionGraph graph) : base(3, graph)
    {
        
    }

    float x { get { return childNodes[0].Evaluate(); } }
    float y { get { return childNodes[1].Evaluate(); } }
    float z { get { return childNodes[2].Evaluate(); } }

    public override string ShortDescription { get { return "3D Simplex Noise"; } }

    public override float Evaluate()
    {
        return SimplexNoise.Evaulate(x,y,z);
    }

    public override int ValidateSelf()
    {
        return 0;
    }
}
