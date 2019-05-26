public class NegativeNode : SingularChildNode
{
    public override string ShortDescription { get { return "-"; } }

    public NegativeNode(FunctionGraph graph) : base(graph)
    {    }

    public override float Evaluate()
    {
        return -Child.Evaluate();
    }

}

