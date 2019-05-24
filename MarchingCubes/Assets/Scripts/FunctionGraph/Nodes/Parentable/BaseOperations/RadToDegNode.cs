using UnityEngine;

public class RadToDegNode : SingularChildNode
{
    public RadToDegNode(FunctionGraph graph) : base(graph)
    {}

    public override float Evaluate()
    {
        return Child.Evaluate() * Mathf.Rad2Deg;
    }
}

