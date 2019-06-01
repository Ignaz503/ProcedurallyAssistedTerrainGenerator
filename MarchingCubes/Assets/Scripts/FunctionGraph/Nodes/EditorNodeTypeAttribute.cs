using System;

[AttributeUsage(AttributeTargets.Class,AllowMultiple = false)]
public class EditorNodeTypeAttribute : Attribute
{
    public enum NodeType
    {
        Default,
        FloatField,
        Variable
    }

    public NodeType Type { get; protected set; }

    public EditorNodeTypeAttribute(NodeType nodeType)
    {
        Type = nodeType;
    }
}
