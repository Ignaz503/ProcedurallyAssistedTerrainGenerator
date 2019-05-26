using UnityEngine;

public class RoundNode : SingularChildNode
{
    public override string ShortDescription { get { return "Round"; } }

    public RoundNode(FunctionGraph graph) : base(graph)
    {
    }

    public override float Evaluate()
    {
        return Mathf.Round(Child.Evaluate());
    }
}

