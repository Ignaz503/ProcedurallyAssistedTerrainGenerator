using UnityEngine;
using UnityEditor;

public class FunctionGraphEditorNodeVariable : FunctionGraphEditorNode
{
    VariableNode n;
    public FunctionGraphEditorNodeVariable(Vector2 creationPosition, BaseFuncGraphNode node, FunctionGraphEditor editor, FunctionGraphEditorNodeLayout layout) : base(creationPosition, node, editor, layout)
    {
        n = node as VariableNode;
    }

    protected override void DrawNode()
    {
        GUI.Box(Rect, "", style);
        //draw text field

        Rect valRect = Rect;

        valRect.position += new Vector2(0f, Rect.size.y * .25f);
        valRect.size = new Vector2(Rect.size.x, Rect.size.y * .5f);

        GUILayout.BeginArea(valRect);

        EditorGUILayout.LabelField("Variable: ");
        n.Var = (FunctionGraph.VariableNames)EditorGUILayout.EnumPopup(n.Var);

        GUILayout.EndArea();
    }
}
