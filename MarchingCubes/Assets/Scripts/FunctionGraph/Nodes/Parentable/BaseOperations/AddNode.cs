/// <summary>
/// Root Node of graph, maybe superfluouse
/// instead of dedicated root node
/// make it a Parentable node and make it markeable as the root
/// meaning: the func graph stores it
/// </summary>
/// <summary>
/// Basic addition Node
/// </summary>
public class AddNode : DualChildNode
{
    public AddNode(FunctionGraph graph) : base(graph)
    {}

    /// <summary>
    /// evaluates the left and right child and ads them togther
    /// </summary>
    /// <returns>LeftChild + RightChild</returns>
    public override float Evaluate()
    {
        return LeftChild.Evaluate() + RightChild.Evaluate();
    }
}