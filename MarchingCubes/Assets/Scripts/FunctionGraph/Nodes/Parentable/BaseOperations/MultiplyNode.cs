using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplyNode : DualChildNode
{
    public MultiplyNode(FunctionGraph graph) : base(graph)
    {
    }

    public override string ShortDescription { get { return "*"; } }

    public override float Evaluate()
    {
        return LeftChild.Evaluate() * RightChild.Evaluate();
    }
}
