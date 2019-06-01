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
}
