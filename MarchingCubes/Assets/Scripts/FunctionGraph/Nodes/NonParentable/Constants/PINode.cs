using System.IO;
using UnityEngine;

public class PINode : FixedConstantNode
{
    public override string ShortDescription { get { return "PI"; } }

    public PINode(FunctionGraph graph) : base(graph)
    {
      
    }

    public override float Evaluate()
    {
        return Mathf.PI;
    }

    public override void Write(StreamWriter writer)
    {
        writer.Write("Mathf.PI ");
    }
}

