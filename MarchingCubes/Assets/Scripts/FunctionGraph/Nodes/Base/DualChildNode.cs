using System.Collections.Generic;
using UnityEngine;

public abstract class DualChildNode : ParentableNode
{
    public BaseFuncGraphNode RightChild { get; protected set; }
    public abstract string RightChildLabel { get; }
    public BaseFuncGraphNode LeftChild { get; protected set; }
    public abstract string LeftChildLabel { get;  }

    public override int PossibleChildrenCount { get { return 2; } }

    public DualChildNode(FunctionGraph graph) : base(graph)
    { }

    public void AddRightChild(BaseFuncGraphNode node)
    {
        if (RightChild != null)
        {
            RightChild.UnsetParent();
        }
        RightChild = node;
        RightChild.SetParent(this);
    }

    public void AddLeftChild(BaseFuncGraphNode node)
    {
        if (LeftChild != null)
        {
            LeftChild.UnsetParent();
        }
        LeftChild = node;
        LeftChild.SetParent(this);
    }

    public void RemoveRightChild()
    {
        if (RightChild != null)
        {
            RightChild.UnsetParent();
            RightChild = null;

        }
    }

    public void RemoveLeftChild()
    {
        if (LeftChild != null)
        {
            LeftChild.UnsetParent();
            LeftChild = null;
        }
    }

    public override void RemoveChild(BaseFuncGraphNode n)
    {
        if (n == RightChild)
            RemoveRightChild();
        else if (n == LeftChild)
            RemoveLeftChild();
    }

    public override int Validate()
    {
        if (RightChild == null)
        {
            FunctionGraphValidationMessageLogger.LogError("Right child is not defined");
            return 1;

        }
        else
        {
            int ret = RightChild.Validate();
            if (ret > 0) return ret;
        }
        if (LeftChild == null)
        {
            FunctionGraphValidationMessageLogger.LogError("Left child is not defined");
            return 1;
        }
        else
        {
            int ret = LeftChild.Validate();
            if (ret > 0) return ret;
        }
        return ValidateSelf();
    }

    protected virtual int ValidateSelf()
    {
        return 0;
    }

    public override void AddChild(int idx, BaseFuncGraphNode n)
    {
        switch (idx)
        {
            case 0:
                AddLeftChild(n);
                break;
            case 1:
                AddRightChild(n);
                break;
            default:
                return;
        }
    }

    public override void RemoveChildAt(int idx)
    {
        switch (idx)
        {
            case 0:
                RemoveLeftChild();
                break;
            case 1:
                RemoveRightChild();
                break;
            default:
                return;
        }
    }

    public override ChildInfo GetChildInfo(BaseFuncGraphNode n)
    {
        if (n == LeftChild)
            return new ChildInfo( true,  0);
        else if (n == RightChild)
            return new ChildInfo( true, 0);
        else
            return new ChildInfo(false, -1);
    }

    public override void RemoveAllChildren()
    {
        RemoveLeftChild();
        RemoveRightChild();
    }

    public override IEnumerator<BaseFuncGraphNode> GetEnumerator()
    {
        for (int i = 0; i < 2; i++)
        {
            switch (i)
            {
                case 0:
                    yield return LeftChild;
                    break;
                case 1:
                    yield return RightChild;
                    break;
            }
        }
    }

}
