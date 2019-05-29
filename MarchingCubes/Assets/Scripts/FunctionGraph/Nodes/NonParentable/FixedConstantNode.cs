public abstract class FixedConstantNode : BaseFuncGraphNode
{
    public FixedConstantNode(FunctionGraph graph) : base(graph)
    {
    }

    public override int Validate()
    {
        return 0;
    }
}