﻿using System.IO;
using FuncGraph.CodeWriting;

public class NegativeNode : SingularChildNode
{
    public override string ShortDescription { get { return "Negate"; } }

    public NegativeNode(FunctionGraph graph) : base(graph)
    {    }

    public override float Evaluate()
    {
        return -Child.Evaluate();
    }

    public override void Write(StreamWriter writer)
    {
        writer.Write("( -");
        Child.Write(writer);
        writer.Write(") ");

    }

    public override void WriteToCSharp(CSharpCodeWriter writer)
    {
        writer.AddToCurrentRHS(" ( -");
        Child.WriteToCSharp(writer);
        writer.AddToCurrentRHS(" )");
    }
}

