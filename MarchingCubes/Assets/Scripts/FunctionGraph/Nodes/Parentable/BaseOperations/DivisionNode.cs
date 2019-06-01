using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DivisionNode : DualChildNode
{
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
}
