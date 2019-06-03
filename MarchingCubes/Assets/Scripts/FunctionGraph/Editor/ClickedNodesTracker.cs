using System;
using UnityEngine;
using UnityEditor;

public struct ClickedNodesTracker
{
    public FunctionGraphEditor Editor;
    ClickedNodeInfo inNode;
    ClickedNodeInfo outNode;

    public void SetInNode(FunctionGraphEditorNode node, ConnectionPoint p, int nodeChildIndx)
    {
        inNode = new ClickedNodeInfo(node, p, nodeChildIndx);

        TryCreateConnection();
    }

    public void SetOutNode(FunctionGraphEditorNode node, ConnectionPoint p, int nodeChildIndx)
    {
        outNode = new ClickedNodeInfo(node, p, nodeChildIndx);

        TryCreateConnection();
    }

    private void TryCreateConnection()
    {
        //TODO
        //if both not null try check if loop
        //if no loop parent out to to in info
        //reset both of them

        if (inNode != null && outNode != null)
        {
            if (!inNode.EditorNode.GraphNode.HasLoopCheck(outNode.EditorNode.GraphNode))
            {
                //there is no loop
                outNode.EditorNode.CreateConnection(inNode.EditorNode, inNode.NodeChildIdx, outNode.ConPoint,inNode.ConPoint);
            }
            else
            {
                Editor.LogWarning("Loop Warning", "Would Create A Loop");
                
            }
            Reset();
        }
    }

    public void Reset()
    {
        inNode = null;
        outNode = null;
        GUI.changed = true;
    }

    public bool IsTracking
    {
        get
        {
            return inNode != null || outNode != null;
        }
    }
        
    public void DrawDummyConnection(Vector2 mousePosition)
    {
        if (inNode != null)
        {
            Draw(inNode, mousePosition);
            GUI.changed = true;
        }
        else if (outNode != null)
        {
            Draw(outNode, mousePosition);
            GUI.changed = true;
        }

    }

    void Draw(ClickedNodeInfo info, Vector2 mousePos)
    {
        Rect startRect = info.ConPoint.OffsetRect;
        startRect.position += info.EditorNode.Rect.position;

        Vector3 startPos = startRect.position + startRect.size * .5f;
        float dirChange = 1f;

        if (startRect.x > mousePos.x)
            dirChange = -1f;

        Handles.DrawBezier(
             mousePos,
             startRect.center,
             mousePos + (Vector2.left* dirChange) * 50f,
             startRect.center - (Vector2.left*dirChange) * 50f,
             Color.white,
             null,
             2f
         );
       
    }

}

public class ClickedNodeInfo
{
    public FunctionGraphEditorNode EditorNode;
    public ConnectionPoint ConPoint;
    public int NodeChildIdx;

    public ClickedNodeInfo(FunctionGraphEditorNode node, ConnectionPoint conPoint, int nodeChildIdx)
    {
        EditorNode = node ?? throw new ArgumentNullException(nameof(node));
        ConPoint = conPoint ?? throw new ArgumentNullException(nameof(conPoint));
        NodeChildIdx = nodeChildIdx;
    }
}


