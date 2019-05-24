using System;
using System.Collections;
using UnityEngine;

public class ConstantNode : BaseFuncGraphNode
{
    [SerializeField] float constant;

    public ConstantNode(FunctionGraph g):base(g)
    {}

    public override float Evaluate()
    {
        return constant;
    }

    public override int Validate()
    {
        return 0;
    }
}
