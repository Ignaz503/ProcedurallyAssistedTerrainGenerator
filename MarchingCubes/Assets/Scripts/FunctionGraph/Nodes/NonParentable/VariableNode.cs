using UnityEngine;

public class VariableNode : BaseFuncGraphNode
{
    public override string ShortDescription { get { return var.ToString(); } }
    [SerializeField] FunctionGraph.VariableNames var;

    public VariableNode(FunctionGraph g): base(g)
    {    }

    public override float Evaluate()
    {
        return Graph.GetVariableValue(var);
        //return parent.GetVariableValue(var);
    }

    public override int Validate()
    {
        return 0;
    }
}
