using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivisionNode : DualChildNode
{
    public DivisionNode(FunctionGraph graph) : base(graph)
    {
    }

    public override string ShortDescription { get { return "Division"; } }

    public override float Evaluate()
    {
        return LeftChild.Evaluate() / RightChild.Evaluate();
    }
}
