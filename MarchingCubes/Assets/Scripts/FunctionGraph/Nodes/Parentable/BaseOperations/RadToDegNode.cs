using System.IO;
using FuncGraph.CodeWriting;
using UnityEngine;

public class RadToDegNode : SingularChildNode
{
    public override string ShortDescription { get { return "RadToDag"; } }

    public RadToDegNode(FunctionGraph graph) : base("f",graph)
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

    //public override void WriteToCSharp(CSharpCodeWriter writer)
    //{
    //    throw new System.NotImplementedException();
    //}
}

