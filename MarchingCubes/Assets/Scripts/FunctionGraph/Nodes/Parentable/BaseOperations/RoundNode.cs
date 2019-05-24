using UnityEngine;

public class RoundNode : SingularChildNode
{
    public RoundNode(FunctionGraph graph) : base(graph)
    {
    }

    public override float Evaluate()
    {
        return Mathf.Round(Child.Evaluate());
    }
}

