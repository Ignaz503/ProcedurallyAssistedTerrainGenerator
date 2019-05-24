using UnityEngine;

public class AbsNode : SingularChildNode
{
    public AbsNode(FunctionGraph graph) : base(graph)
    {
    }

    public override float Evaluate()
    {
        return Mathf.Abs(Child.Evaluate());
    }

}

