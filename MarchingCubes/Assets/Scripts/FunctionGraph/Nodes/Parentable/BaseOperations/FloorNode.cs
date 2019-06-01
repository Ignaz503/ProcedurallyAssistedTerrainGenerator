using System.IO;
using UnityEngine;

public class FloorNode : SingularChildNode
{
    public FloorNode(FunctionGraph graph) : base(graph)
    {
    }

    public override string ShortDescription { get { return "Floor"; } }

    public override float Evaluate()
    {
        return Mathf.Floor(Child.Evaluate());
    }

    public override void Write(StreamWriter writer)
    {
        writer.Write("Mathf.Floor( ");
        Child.Write(writer);
        writer.Write(") ");
    }
}

