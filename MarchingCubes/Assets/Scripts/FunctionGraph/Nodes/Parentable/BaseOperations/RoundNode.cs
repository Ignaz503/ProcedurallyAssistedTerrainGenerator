using System.IO;
using UnityEngine;

public class RoundNode : SingularChildNode
{
    public override string ShortDescription { get { return "Round"; } }

    public RoundNode(FunctionGraph graph) : base(graph)
    {
    }

    public override float Evaluate()
    {
        return Mathf.Round(Child.Evaluate());
    }

    public override void Write(StreamWriter writer)
    {
        writer.Write("Mathf.Round( ");
        Child.Write(writer);
        writer.Write(") ");
    }

}

