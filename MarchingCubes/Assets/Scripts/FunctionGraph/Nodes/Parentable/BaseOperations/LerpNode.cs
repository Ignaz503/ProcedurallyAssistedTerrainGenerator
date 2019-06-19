﻿using System.IO;
using FuncGraph.CodeWriting;
using UnityEngine;

public class LerpNode : FixedSizeMultiChildNode
{
    public override string ShortDescription { get { return "Lerp"; } }

    public LerpNode(FunctionGraph graph) : base(3, graph)
    {
    }

    /// <summary>
    /// Linear interpolation between child node Mathf.Lerp(child[0],child[1],child[2])
    /// 
    /// </summary>
    /// <returns></returns>
    public override float Evaluate()
    {
        return Mathf.Lerp(childNodes[0].Evaluate(), childNodes[1].Evaluate(), childNodes[2].Evaluate());
    }

    public override int ValidateSelf()
    {
        return 0;
    }

    public override void Write(StreamWriter writer)
    {
        writer.Write("Mathf.Lerp( ");
        childNodes[0].Write(writer);
        writer.Write(", ");
        childNodes[1].Write(writer);
        writer.Write(", ");
        childNodes[2].Write(writer);
        writer.Write(") ");
    }

    public override void WriteToCSharp(CSharpCodeWriter writer)
    {
        writer.AddToCurrentRHS(" Mathf.Lerp(");
        childNodes[0].WriteToCSharp(writer);
        writer.AddToCurrentRHS(",");
        childNodes[1].WriteToCSharp(writer);
        writer.AddToCurrentRHS(",");
        childNodes[2].WriteToCSharp(writer);
        writer.AddToCurrentRHS(" )");
    }
}