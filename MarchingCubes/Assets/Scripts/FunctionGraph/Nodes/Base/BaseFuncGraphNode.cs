using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseFuncGraphNode : IFuncGraphNode
{
    //TODO added function for drawing in editor and so on
    //same as eventHandling 

    public event Action<BaseFuncGraphNode> OnUncoupleFromParent;
    public FunctionGraph Graph { get; protected set; }
    protected ParentableNode parent;
    public virtual int PossibleChildrenCount{ get { return 0; } }

    protected BaseFuncGraphNode(FunctionGraph graph)
    {
        this.Graph = graph ?? throw new ArgumentNullException(nameof(graph));
    }

    public abstract float Evaluate();
    public abstract int Validate();

    public void SetParent(ParentableNode n)
    {
        parent = n;
    }

    public void UnsetParent() {
        parent = null;
    }

    public void ParentTo(ParentableNode n, int idx)
    {
        if (checkSelfReference(n))
            return;
        if (parent != null)
            UncoupleFromParent();
        n.AddChild(idx, this);
    }

    public void ParentTo(BaseFuncGraphNode n, int idx)
    {

        if (checkSelfReference(n))
            return;
        if (parent != null)
            UncoupleFromParent();
        if (n is ParentableNode)
        {
            var pN = n as ParentableNode;
            pN.AddChild(idx, this);
        }
    }

    public void UncoupleFromParent()
    {
        if (parent != null)
        {
            Debug.Log("Uncoupling");
            parent.RemoveChild(this);
            OnUncoupleFromParent?.Invoke(this);
        }
    }

    bool checkSelfReference(BaseFuncGraphNode n)
    {
        return n == this;
    }

    public bool LoopCheck(BaseFuncGraphNode potentialCild)
    {
        return LoopCheckVisitParent(potentialCild);
    }

    protected virtual bool LoopCheckVisitParent(BaseFuncGraphNode potentialCild)
    {
        if (potentialCild == parent)
            return true;
        else if (parent != null)
            return parent.LoopCheck(potentialCild);
        else
            return false;
    }

    public bool HasParent
    {
        get { return parent != null; }
    }

}
