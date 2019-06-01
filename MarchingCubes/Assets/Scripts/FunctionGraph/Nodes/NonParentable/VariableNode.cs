using System.IO;
using UnityEngine;

[EditorNodeType(EditorNodeTypeAttribute.NodeType.Variable)]
public class VariableNode : BaseFuncGraphNode
{
    public override string ShortDescription { get { return "Variable"; } }
    public FunctionGraph.VariableNames Var;

    public VariableNode(FunctionGraph g): base(g)
    {    }

    public override float Evaluate()
    {
        return Graph.GetVariableValue(Var);
    }

    public override int Validate()
    {
        return 0;
    }

    /// <summary>
    /// public interface IDensityFunc
    ///float Evaluate(SamplePointVariables x, SamplePointVariables y, SamplePointVariables z);
    /// SamplePointsVariablse
    /// {
    ///     public float ValueWorld;
    ///     public float ValueLocal;
    /// }
    /// </summary>
    /// <param name="writer"></param>
    public override void Write(StreamWriter writer)
    {
        switch (Var)
        {
            case FunctionGraph.VariableNames.X:
                writer.Write("x.ValueWorld");
                break;
            case FunctionGraph.VariableNames.XLocal:
                writer.Write("x.ValueLocal");
                break;
            case FunctionGraph.VariableNames.Y:
                writer.Write("y.ValueWorld");
                break;
            case FunctionGraph.VariableNames.YLocal:
                writer.Write("y.ValueLocal");
                break;
            case FunctionGraph.VariableNames.Z:
                writer.Write("z.ValueWorld");
                break;
            case FunctionGraph.VariableNames.ZLocal:
                writer.Write("z.ValueLocal");
                break;
        }
        writer.Write(" ");
    }
}
