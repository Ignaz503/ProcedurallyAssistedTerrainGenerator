using UnityEngine;

public class CeilNode : SingularChildNode
{
    public override string ShortDescription { get { return "Ceil"; } }

    public CeilNode(FunctionGraph graph) : base(graph)
    {
}

    public override float Evaluate()
    {
        return Mathf.Ceil(Child.Evaluate());
    }
}

