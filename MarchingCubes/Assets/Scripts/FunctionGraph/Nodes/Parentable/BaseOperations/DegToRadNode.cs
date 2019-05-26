using UnityEngine;

public class DegToRadNode : SingularChildNode
{
    public override string ShortDescription { get { return "DegToRad"; } }

    public DegToRadNode(FunctionGraph graph) : base(graph)
    {
    }

    public override float Evaluate()
    {
        return Child.Evaluate() * Mathf.Deg2Rad;
    }
}

