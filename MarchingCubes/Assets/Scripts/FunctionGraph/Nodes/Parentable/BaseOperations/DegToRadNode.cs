using System.IO;
using FuncGraph.CodeWriting;
using UnityEngine;

public class DegToRadNode : SingularChildNode
{
    public override string ShortDescription { get { return "DegToRad"; } }

    public DegToRadNode(FunctionGraph graph) : base("f",graph)
    {
    }

    public override float Evaluate()
    {
        return Child.Evaluate() * Mathf.Deg2Rad;
    }

    public override void Write(StreamWriter writer)
    {
        writer.Write("( ");
        Child.Write(writer);
        writer.Write("*  Mathf.Deg2Rad ");
        writer.Write(") ");
    }

    //public override void WriteToCSharp(CSharpCodeWriter writer)
    //{
    //    throw new System.NotImplementedException();
    //}
}

