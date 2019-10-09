using System.Collections.Generic;

public abstract class FixedSizeMultiChildNode : ParentableNode
{
    protected BaseFuncGraphNode[] childNodes;
    protected string[] childNodeLabels;
    public IEnumerable<BaseFuncGraphNode> ChildNodes { get { return childNodes; } }
    public int Size { get { return childNodes.Length; } }
    public override int PossibleChildrenCount { get { return Size; } }

    protected FixedSizeMultiChildNode(int size,string[] labels, FunctionGraph graph) : base(graph)
    {
        if (labels.Length != size)
            throw new System.Exception("Labels need to be same size as possible children");
        childNodes = new BaseFuncGraphNode[size];
        childNodeLabels = labels;
    }

    public override string GetChildLabelByIdx(int idx)
    {
        if (idx < Size)
        {
            return childNodeLabels[idx];
        }
        return "Invalid Child Idx";
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

    public override void RemoveAllChildren()
    {
        for (int i = 0; i < Size; i++)
        {
            RemoveChildAt(i);
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

    public override IEnumerator<BaseFuncGraphNode> GetEnumerator()
    {
        for (int i = 0; i < Size; i++)
        {
            yield return childNodes[i];
        }
    }
}