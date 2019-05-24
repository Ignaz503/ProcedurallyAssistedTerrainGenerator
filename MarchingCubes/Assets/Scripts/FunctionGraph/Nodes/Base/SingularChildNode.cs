using System.Collections.Generic;

public abstract class SingularChildNode : ParentableNode
{
    public BaseFuncGraphNode Child { get; protected set; }
    public override int PossibleChildrenCount { get { return 1; } }
    protected SingularChildNode(FunctionGraph graph) : base(graph)
    {}

    public override void AddChild(int idx, BaseFuncGraphNode n)
    {
        if (Child != null)
        {
            Child.UnsetParent();
        }
        Child = n;
        n.SetParent(this);
    }

    public override void RemoveChild(BaseFuncGraphNode n)
    {
        if (Child != null && n == Child)
        {
            //trying to remvoe actual child
            Child.UnsetParent();
            Child = null;
        }
    }

    public override void RemoveChildAt(int idx)
    {

        if (Child != null)
        {
            Child.UnsetParent();
            Child = null;
        }
    }

    public override void RemoveAllChildren()
    {
        RemoveChildAt(0);
    }

    public override int Validate()
    {
        if (Child == null)
        {
            FunctionGraphValidationMessageLogger.LogError("No Child defined");
            return 1;
        }
        return ValidateSelf();
    }

    protected virtual int ValidateSelf()
    {
        return 0;
    }

    public override ChildInfo GetChildInfo(BaseFuncGraphNode n)
    {
        if (n == Child)
            return new ChildInfo( true, 0);
        else
            return new ChildInfo(false, -1 );
    }

    public override IEnumerator<BaseFuncGraphNode> GetEnumerator()
    {
        yield return Child;
    }

}