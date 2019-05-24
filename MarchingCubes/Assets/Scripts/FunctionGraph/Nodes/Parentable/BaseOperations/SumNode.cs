public class SumNode : VariableMultiChildNode
{
    public SumNode(FunctionGraph graph) : base(graph)
    {
    }

    public override float Evaluate()
    {
        float val = 0;
        for (int i = 0; i < children.Count; i++)
        {
            val += children[i].Evaluate();
        }
        return val;
    }
}

