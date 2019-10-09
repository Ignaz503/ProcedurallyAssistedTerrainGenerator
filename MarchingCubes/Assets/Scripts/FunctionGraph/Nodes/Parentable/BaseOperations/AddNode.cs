using System.IO;
using FuncGraph.CodeWriting;
/// <summary>
/// Root Node of graph, maybe superfluouse
/// instead of dedicated root node
/// make it a Parentable node and make it markeable as the root
/// meaning: the func graph stores it
/// </summary>
/// <summary>
/// Basic addition Node
/// </summary>
public class AddNode : DualChildNode
{
    public override string RightChildLabel { get { return "B"; } }
    public override string LeftChildLabel { get { return "A"; } }

    public override string ShortDescription { get { return "+"; } }

    public AddNode(FunctionGraph graph) : base(graph)
    {}

    /// <summary>
    /// evaluates the left and right child and ads them togther
    /// </summary>
    /// <returns>LeftChild + RightChild</returns>
    public override float Evaluate()
    {
        return LeftChild.Evaluate() + RightChild.Evaluate();
    }

    public override void Write(StreamWriter writer)
    {
        writer.Write("( ");
        LeftChild.Write(writer);
        writer.Write("+ ");
        RightChild.Write(writer);
        writer.Write(") ");
    }

    public override void WriteToCSharp(CSharpCodeWriter writer)
    {
        writer.AddToCurrentLine(" (");
        LeftChild.WriteToCSharp(writer);
        writer.AddToCurrentLine(" +");
        RightChild.WriteToCSharp(writer);
        writer.AddToCurrentLine(" )");
    }
}