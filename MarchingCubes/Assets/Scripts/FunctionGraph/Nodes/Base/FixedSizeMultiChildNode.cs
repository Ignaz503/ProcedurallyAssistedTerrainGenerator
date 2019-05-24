using System.Collections.Generic;

public abstract class FixedSizeMultiChildNode : ParentableNode
{
    protected BaseFuncGraphNode[] childNodes;
    public IEnumerable<BaseFuncGraphNode> ChildNodes { get { return childNodes; } }
    public int Size { get { return childNodes.Length; } }
    public override int PossibleChildrenCount { get { return Size; } }

    protected FixedSizeMultiChildNode(int size, FunctionGraph graph) : base(graph)
    {
        childNodes = new BaseFuncGraphNode[size];
    }

    public override int Validate()
    {
        for (int i = 0; i < childNodes.Length; i++)
        {
            if (childNodes[i] == null)
            {
                FunctionGraphValidationMessageLogger.LogError($"Child {i} is undefined");
                return 1;
            }
            int ret = childNodes[i].Validate();
            if (ret > 0) return ret;
        }
        return ValidateSelf();
    }

    public override void RemoveChild(BaseFuncGraphNode n)
    {
        for (int i = 0; i < childNodes.Length; i++)
        {
            if (childNodes[i] == n)
            {
                n.UnsetParent();
                childNodes[i] = null;
                break;
            }
        }
    }

   
    public override void RemoveChildAt(int idx)
    {
        if (idx < childNodes.Length && childNodes[idx] != null)
        {
            childNodes[idx].UnsetParent();
            childNodes[idx] = null;
        }
    }

    public override void AddChild(int idx, BaseFuncGraphNode n)
    {
        if (idx < childNodes.Length)
        {
            if (childNodes[idx] != null)
                childNodes[idx].UnsetParent();
            childNodes[idx] = n;
            n.SetParent(this);
        }
    }

    public abstract int ValidateSelf();

    public override ChildInfo GetChildInfo(BaseFuncGraphNode n)
    {
        for (int i = 0; i < Size; i++)
        {
            if (n == childNodes[i])
            {
                return new ChildInfo( true,  i );
            }
        }
        return new ChildInfo( false, -1 );
    }
}