using System;
using UnityEngine;
using UnityEditor;

[Serializable]
public class ConnectionToDraw
{
    public FunctionGraphEditorNode fromNode;
    public FunctionGraphEditorNode toNode;

    public ConnectionPoint fromPoint;
    public ConnectionPoint toPoint;

    public int EditorNodeConnectionPointIndex;

    public ConnectionToDraw(FunctionGraphEditorNode fromNode, FunctionGraphEditorNode toNode, ConnectionPoint fromPoint, ConnectionPoint toPoint, int editorNodeConnectionPointIndex)
    {
        this.fromNode = fromNode ?? throw new ArgumentNullException(nameof(fromNode));
        this.toNode = toNode ?? throw new ArgumentNullException(nameof(toNode));
        this.fromPoint = fromPoint ?? throw new ArgumentNullException(nameof(fromPoint));
        this.toPoint = toPoint ?? throw new ArgumentNullException(nameof(toPoint));
        EditorNodeConnectionPointIndex = editorNodeConnectionPointIndex;
    }

    public void Draw()
    {

        Rect startRect = CalculateRectFromOffsetRectAndParent(fromNode.Rect, fromPoint.OffsetRect);
        Rect endRect = CalculateRectFromOffsetRectAndParent(toNode.Rect, toPoint.OffsetRect);

        Vector3 startPos = startRect.position + startRect.size * .5f;
        Vector3 endPos = endRect.position + endRect.size * .5f;

        float dirChange = 1f;
        if (startRect.x > endPos.x)
            dirChange = -1f;


        Handles.DrawBezier(
             endRect.center,
             startRect.center,
             endRect.center + (Vector2.left*dirChange) * 50f,
             startRect.center - (Vector2.left*dirChange) * 50f,
             Color.white,
             null,
             2f
         );
        DrawHandle(startPos,endPos);
       
    }

    Rect CalculateRectFromOffsetRectAndParent(Rect parent, Rect offsetRect)
    {
        Rect r = offsetRect;
        r.position += parent.position;
        return r;
    }

    void DrawHandle(Vector2 startPos, Vector2 endPos)
    {
        if (Handles.Button(Vector3.Lerp(startPos, endPos, .5f), Quaternion.identity, 4f, 8f, Handles.RectangleHandleCap))     
        {
            fromNode.DeleteConnection();
        }
    }

}
