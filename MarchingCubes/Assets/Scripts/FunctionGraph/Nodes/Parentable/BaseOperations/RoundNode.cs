using System.IO;
using FuncGraph.CodeWriting;
using UnityEngine;

public class RoundNode : SingularChildNode
{
    public override string ShortDescription { get { return "Round"; } }

    public RoundNode(FunctionGraph graph) : base("f", graph)
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

    public override void WriteToCSharp(CSharpCodeWriter writer)
    {
        writer.CurrentLine.Append("Mathf.Round( ");
        Child.WriteToCSharp(writer);
        writer.CurrentLine.Append(") ");
    }
}

