using UnityEngine;

public class PINode : FixedConstantNode
{
    public override string ShortDescription { get { return "PI"; } }

    public PINode(FunctionGraph graph) : base(graph)
    {
      
    }

    public override float Evaluate()
    {
        return Mathf.PI;
    }

}

