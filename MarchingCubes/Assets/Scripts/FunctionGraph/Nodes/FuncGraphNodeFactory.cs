using System;
using System.Reflection;

public static class FuncGraphNodeFactory
{
    public static BaseFuncGraphNode CreateNode(Type t, FunctionGraph graph)
    {
        return Activator.CreateInstance(t, graph) as BaseFuncGraphNode;
    }
}