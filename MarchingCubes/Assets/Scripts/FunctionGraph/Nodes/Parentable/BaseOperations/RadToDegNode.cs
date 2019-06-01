using System.IO;
using UnityEngine;

public class RadToDegNode : SingularChildNode
{
    public override string ShortDescription { get { return "RadToDag"; } }

    public RadToDegNode(FunctionGraph graph) : base(graph)
    {}

    public override float Evaluate()
    {
        return Child.Evaluate() * Mathf.Rad2Deg;
    }

    public override void Write(StreamWriter writer)
    {
        writer.Write("( ");
        Child.Write(writer);
        writer.Write("*  Mathf.Rad2Deg ");
        writer.Write(") ");
    }
}

