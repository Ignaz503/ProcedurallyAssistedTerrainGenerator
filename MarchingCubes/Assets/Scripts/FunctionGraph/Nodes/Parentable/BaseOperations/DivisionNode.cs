﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using FuncGraph.CodeWriting;
using UnityEngine;

public class DivisionNode : DualChildNode
{
    public override string RightChildLabel { get { return "B"; } }
    public override string LeftChildLabel { get { return "A"; } }

    public DivisionNode(FunctionGraph graph) : base(graph)
    {
    }

    public override string ShortDescription { get { return "Division"; } }

    public override float Evaluate()
    {
        return LeftChild.Evaluate() / RightChild.Evaluate();
    }

    public override void Write(StreamWriter writer)
    {
        writer.Write("( ");
        LeftChild.Write(writer);
        writer.Write("/ ");
        RightChild.Write(writer);
        writer.Write(") ");
    }

    public override void WriteToCSharp(CSharpCodeWriter writer)
    {
        writer.CurrentLine.Append("( ");
        LeftChild.WriteToCSharp(writer);
        writer.CurrentLine.Append("/ ");
        RightChild.WriteToCSharp(writer);
        writer.CurrentLine.Append(") ");
    }
}
