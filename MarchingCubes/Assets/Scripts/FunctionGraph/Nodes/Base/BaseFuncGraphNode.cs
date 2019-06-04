using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using System.IO;

public abstract class BaseFuncGraphNode : IFuncGraphNode
{
    static IEnumerable<Type> _instantiableTypes;
    public static IEnumerable<Type> InstantiableNodeTypes
    {
        get
        {
            if(_instantiableTypes == null)
                _instantiableTypes =  Assembly.GetAssembly(typeof(BaseFuncGraphNode)).GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(BaseFuncGraphNode)));
            return _instantiableTypes;
        }
    }

    //TODO added function for drawing in editor and so on
    //same as eventHandling 

    public event Action<BaseFuncGraphNode> OnUncoupleFromParent;
    public FunctionGraph Graph { get; protected set; }
    protected ParentableNode parent;
    public ParentableNode Parent { get { return parent; } }
    public virtual int PossibleChildrenCount{ get { return 0; } }
    public abstract string ShortDescription { get; }

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
            parent.RemoveChild(this);
            OnUncoupleFromParent?.Invoke(this);
        }
    }

    bool checkSelfReference(BaseFuncGraphNode n)
    {
        return n == this;
    }

    public bool HasLoopCheck(BaseFuncGraphNode potentialCild)
    {
        return HasLoopCheckVisitParent(potentialCild);
    }

    protected virtual bool HasLoopCheckVisitParent(BaseFuncGraphNode potentialCild)
    {
        if (potentialCild == parent)
            return true;
        else if (parent != null)
            return parent.HasLoopCheck(potentialCild);
        else
            return false;
    }

    public bool HasParent
    {
        get { return parent != null; }
    }

    public abstract void Write(StreamWriter writer);
}
