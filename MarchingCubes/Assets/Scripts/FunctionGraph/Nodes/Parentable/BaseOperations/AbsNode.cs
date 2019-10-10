using System.IO;
using FuncGraph.CodeWriting;
using UnityEngine;

public class AbsNode : SingularChildNode
{
    public override string ShortDescription { get { return "Abs"; } }

    public AbsNode(FunctionGraph graph) : base("f", graph)
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

    public override void WriteToCSharp(CSharpCodeWriter writer)
    {
        writer.CurrentLine.Append(nameof(Mathf) + "." + nameof(Mathf.Abs)+"(");
        Child.WriteToCSharp(writer);
        writer.CurrentLine.Append(")");
    }
}

