public class NegativeNode : SingularChildNode
{
    public NegativeNode(FunctionGraph graph) : base(graph)
    {    }

    public override float Evaluate()
    {
        return -Child.Evaluate();
    }

}

