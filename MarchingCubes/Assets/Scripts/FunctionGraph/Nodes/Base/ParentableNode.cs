public abstract class ParentableNode : BaseFuncGraphNode
{
    protected ParentableNode(FunctionGraph graph) : base(graph)
    {}

    public abstract void RemoveChild(BaseFuncGraphNode n);

    public abstract void RemoveChildAt(int idx);

    public abstract void AddChild(int idx, BaseFuncGraphNode n);

    public abstract ChildInfo GetChildInfo(BaseFuncGraphNode n);
}
