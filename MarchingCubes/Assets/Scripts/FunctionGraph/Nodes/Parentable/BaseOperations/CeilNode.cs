using System.IO;
using FuncGraph.CodeWriting;
using UnityEngine;

public class CeilNode : SingularChildNode
{
    public override string ShortDescription { get { return "Ceil"; } }

    public CeilNode(FunctionGraph graph) : base("f",graph)
    {
}

    public override float Evaluate()
    {
        return Mathf.Ceil(Child.Evaluate());
    }

    public override void Write(StreamWriter writer)
    {
        writer.Write("Mathf.Ceil( ");
        Child.Write(writer);
        writer.Write(") ");
    }

    public override void WriteToCSharp(CSharpCodeWriter writer)
    {
        writer.AddToCurrentLine(" Mathf.Ceil(");
        Child.WriteToCSharp(writer);
        writer.AddToCurrentLine(" )");
    }
}

