using System.IO;
using FuncGraph.CodeWriting;

public class SumNode : VariableMultiChildNode
{
    public SumNode(FunctionGraph graph) : base(graph)
    {
    }

    public override string ShortDescription { get { return "Sum"; } }

    public override float Evaluate()
    {
        float val = 0;
        for (int i = 0; i < children.Count; i++)
        {
            val += children[i].Evaluate();
        }
        return val;
    }

    public override void Write(StreamWriter writer)
    {
        writer.Write("( ");
        children[0].Write(writer);

        for (int i = 1; i < children.Count; i++)
        {
            writer.Write(" + ");
            children[i].Write(writer);
        }
        writer.Write(") ");
    }

    public override void WriteToCSharp(CSharpCodeWriter writer)
    {
        writer.AddToCurrentRHS(" (");
        children[0].WriteToCSharp(writer);

        for (int i = 1; i < children.Count; i++)
        {
            writer.AddToCurrentRHS(" +");
            children[i].WriteToCSharp(writer);
        }
        writer.AddToCurrentRHS(" )");
    }
}

