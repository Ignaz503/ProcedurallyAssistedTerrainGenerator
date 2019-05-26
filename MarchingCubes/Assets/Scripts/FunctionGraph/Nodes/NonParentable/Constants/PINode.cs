using UnityEngine;

public class PINode : BaseFuncGraphNode
{
    public override string ShortDescription { get { return "PI"; } }

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

