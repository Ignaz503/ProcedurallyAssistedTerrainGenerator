using System.IO;
using UnityEngine;

public class AbsNode : SingularChildNode
{
    public override string ShortDescription { get { return "Abs"; } }

    public AbsNode(FunctionGraph graph) : base(graph)
    {
    }

    public override float Evaluate()
    {
        return Mathf.Abs(Child.Evaluate());
    }

    public override void Write(StreamWriter writer)
    {
        writer.Write("Mathf.Abs( ");
        Child.Write(writer);
        writer.Write(") ");
    }

}

