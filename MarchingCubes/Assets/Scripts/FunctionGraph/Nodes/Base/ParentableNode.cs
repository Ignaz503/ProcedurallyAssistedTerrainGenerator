using System.Collections;
using System.Collections.Generic;


public abstract class ParentableNode : BaseFuncGraphNode, IEnumerable<BaseFuncGraphNode>
{
    protected ParentableNode(FunctionGraph graph) : base(graph)
    {}

    public abstract void RemoveChild(BaseFuncGraphNode n);

    public abstract void RemoveAllChildren();

    public abstract void RemoveChildAt(int idx);

    public abstract void AddChild(int idx, BaseFuncGraphNode n);

    public abstract ChildInfo GetChildInfo(BaseFuncGraphNode n);

    public bool MoveNext()
    {
        throw new System.NotImplementedException();
    }

    public void Reset()
    {
        throw new System.NotImplementedException();
    }

    public void Dispose()
    {
        throw new System.NotImplementedException();
    }

    public abstract IEnumerator<BaseFuncGraphNode> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new System.NotImplementedException();
    }
}
