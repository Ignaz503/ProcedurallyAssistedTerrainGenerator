using System.Collections.Generic;

public abstract class VariableMultiChildNode : ParentableNode
{
    protected List<BaseFuncGraphNode> children { get; set; }
    protected string childLabel;
    public IEnumerable<BaseFuncGraphNode> Children { get { return children; } }
    public override int PossibleChildrenCount { get { return int.MaxValue; } }

    protected VariableMultiChildNode(string label,FunctionGraph graph) : base(graph)
    {
        children = new List<BaseFuncGraphNode>();
        childLabel = label;
    }

    public override string GetChildLabelByIdx(int idx)
    {
        return childLabel;
    }

    public void AddChild(BaseFuncGraphNode newChild)
    {
        children.Add(newChild);
        newChild.SetParent(this);
    }

    public override void RemoveChild(BaseFuncGraphNode childToRemove)
    {
        if (children.Contains(childToRemove))
        {
            childToRemove.UnsetParent();
            children.Remove(childToRemove);
        }
    }

    public override void RemoveChildAt(int idx)
    {
        if (idx < children.Count && children[idx] != null)
        {
            children[idx].UnsetParent();
            children.RemoveAt(idx);
        }
    }

    public override void RemoveAllChildren()
    {
        for (int i = children.Count; i >= 0; i--)
        {
            RemoveChildAt(i);
        }
    }

    public override void AddChild(int idx, BaseFuncGraphNode n)
    { 
        if (children != null && idx < children.Count)
        {
            var curNode = children[idx];
            if (curNode != null)
                curNode.UnsetParent();

            children[idx] = n;
            n.SetParent(this);
        }
        else if (children != null && idx > children.Count)
        {
            // add diference as null set child
            for (int i = children.Count; i < idx - children.Count; i++)
            {
                children.Add(null);
            }
            children.Add(n);
            n.SetParent(this);
        }
        else
        {
            children.Add(n);
            n.SetParent(this);
        }
    }

    public override int Validate()
    {
        if (children == null || children.Count == 0)
        {
            FunctionGraphValidationMessageLogger.LogError("There are no children defined");
        }
        else
        {
            for (int i = children.Count - 1 ; i >= 0; i--)
            {
                if (children[i] == null) {
                    children.RemoveAt(i);
                    continue;
                }
                int ret = children[i].Validate();
                if (ret > 0) return ret;
            }
        }
        return ValidateSelf();
    }

    protected virtual int ValidateSelf()
    {
        return 0;
    }

    public override ChildInfo GetChildInfo(BaseFuncGraphNode n)
    {
        if (children == null || children.Count == 0)
            return new ChildInfo( false,  -1 );
        else
        {
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i] == n)
                {
                    return new ChildInfo( true, i );
                }
            }
            return new ChildInfo( false,  -1 );
        }
    }

    public override IEnumerator<BaseFuncGraphNode> GetEnumerator()
    {
        for (int i = 0; i < children.Count; i++)
        {
            yield return children[i];
        }
    }
}
