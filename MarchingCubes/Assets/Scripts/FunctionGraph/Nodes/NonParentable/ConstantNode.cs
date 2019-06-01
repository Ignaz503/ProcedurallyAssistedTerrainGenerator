using System;
using System.Collections;
using UnityEngine;

public class ConstantNode : BaseFuncGraphNode
{
    public override string ShortDescription { get { return "Const"; } }

    public float Constant;

    public ConstantNode(FunctionGraph g):base(g)
    {}

    public override float Evaluate()
    {
        return Constant;
    }

    public override int Validate()
    {
        return 0;
    }
}
