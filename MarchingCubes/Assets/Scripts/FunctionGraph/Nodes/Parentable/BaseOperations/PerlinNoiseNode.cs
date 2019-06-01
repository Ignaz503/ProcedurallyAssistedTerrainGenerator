using System.IO;
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

    public override void Write(StreamWriter writer)
    {
        writer.Write("Mathf.PerlinNoise( ");
        LeftChild.Write(writer);
        writer.Write(", ");
        RightChild.Write(writer);
        writer.Write(") ");
    }
}

