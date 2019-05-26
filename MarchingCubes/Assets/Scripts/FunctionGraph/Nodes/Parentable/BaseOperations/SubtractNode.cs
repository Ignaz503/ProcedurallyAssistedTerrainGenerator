using System.Collections;
using System.Collections.Generic;

public class SubtractNode : DualChildNode
{
    public override string ShortDescription { get { return "-"; } }

    public SubtractNode(FunctionGraph graph) : base(graph)
    {}

    /// <summary>
    /// Calculates A - B
    /// </summary>
    /// <returns>LeftChild.Evaluate - RightChild.Evaluate</returns>
    public override float Evaluate()
    {
        return LeftChild.Evaluate() - RightChild.Evaluate();
    }

}
