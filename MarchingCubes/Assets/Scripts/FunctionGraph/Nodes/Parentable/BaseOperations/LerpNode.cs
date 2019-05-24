using UnityEngine;

public class LerpNode : FixedSizeMultiChildNode
{
    public LerpNode(FunctionGraph graph) : base(3, graph)
    {
    }

    /// <summary>
    /// Linear interpolation between child node Mathf.Lerp(child[0],child[1],child[2])
    /// 
    /// </summary>
    /// <returns></returns>
    public override float Evaluate()
    {
        return Mathf.Lerp(childNodes[0].Evaluate(), childNodes[1].Evaluate(), childNodes[2].Evaluate());
    }

    public override int ValidateSelf()
    {
        return 0;
    }
}