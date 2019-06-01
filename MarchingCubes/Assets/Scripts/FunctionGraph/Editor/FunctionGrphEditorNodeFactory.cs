using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class FunctionGraphEditorNodeFactory
{
    public static FunctionGraphEditorNode CreateEditorNode(Vector2 pos, BaseFuncGraphNode n,FunctionGraphEditor editor, FunctionGraphEditorNodeLayout layout)
    {
        var attr = n.GetType().GetCustomAttributes(typeof(EditorNodeTypeAttribute), true) as EditorNodeTypeAttribute[];

        EditorNodeTypeAttribute.NodeType type = EditorNodeTypeAttribute.NodeType.Default;

        if (attr.Length > 0)
        {
            type = attr[0].Type;
        }
       
        switch (type)
        {
            case EditorNodeTypeAttribute.NodeType.Default:
                return new FunctionGraphEditorNode(pos, n, editor, layout);
            case EditorNodeTypeAttribute.NodeType.FloatField:
                return new FunctionGraphEditorNodeConstant(pos, n, editor, layout);
            case EditorNodeTypeAttribute.NodeType.Variable:
                return new FunctionGraphEditorNodeVariable(pos, n, editor, layout);
            default:
                return new FunctionGraphEditorNode(pos, n, editor, layout);

        }
    }
}
