using UnityEngine;

public class FloorNode : SingularChildNode
{
    public FloorNode(FunctionGraph graph) : base(graph)
    {
    }

    public override float Evaluate()
    {
        return Mathf.Floor(Child.Evaluate());
    }
}

