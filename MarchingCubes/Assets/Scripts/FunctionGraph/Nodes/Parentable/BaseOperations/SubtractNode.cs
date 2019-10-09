using System.Collections;
using System.Collections.Generic;
using System.IO;
using FuncGraph.CodeWriting;

public class SubtractNode : DualChildNode
{
    public override string ShortDescription { get { return "-"; } }

    public override string RightChildLabel { get { return "B"; } }
    public override string LeftChildLabel { get { return "A"; } }
    
    public SubtractNode(FunctionGraph graph) : base(graph)
    {}

    /// <summary>
    /// Calculates A - B
    /// </summary>
    /// <returns>LeftChild.Evaluate - RightChild.Evaluate</returns>
    public override float Evaluate()
    {
        return LeftChild.Evaluate() - RightChild.Evaluate();
    }

    public override void Write(StreamWriter writer)
    {
        writer.Write("( ");
        LeftChild.Write(writer);
        writer.Write("- ");
        RightChild.Write(writer);
        writer.Write(") ");
    }

    public override void WriteToCSharp(CSharpCodeWriter writer)
    {
        writer.AddToCurrentLine(" (");
        LeftChild.WriteToCSharp(writer);
        writer.AddToCurrentLine(" -");
        RightChild.WriteToCSharp(writer);
        writer.AddToCurrentLine(" )");
    }
}
