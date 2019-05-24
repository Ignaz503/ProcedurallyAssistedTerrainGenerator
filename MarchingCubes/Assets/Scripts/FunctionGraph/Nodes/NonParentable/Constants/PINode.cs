using UnityEngine;

public class PINode : BaseFuncGraphNode
{
    public PINode(FunctionGraph graph) : base(graph)
    {
    }

    public override float Evaluate()
    {
        return Mathf.PI;
    }

    public override int Validate()
    {
        return 0;
    }
}

