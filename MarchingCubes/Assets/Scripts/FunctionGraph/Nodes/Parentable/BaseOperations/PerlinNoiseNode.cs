using UnityEngine;

public class PerlinNoiseNode : DualChildNode
{
    public PerlinNoiseNode(FunctionGraph graph) : base(graph)
    {
    }

    public override string ShortDescription { get { return "PerlinNoise"; } }

    /// <summary>
    /// calculates Perlin Noise using left child as x and right child as y
    /// </summary>
    /// <returns>Mathf.PlerlinNoise(LC.Eval,RC.Eval)</returns>
    public override float Evaluate()
    {
        return Mathf.PerlinNoise(LeftChild.Evaluate(), RightChild.Evaluate());
    }
}

