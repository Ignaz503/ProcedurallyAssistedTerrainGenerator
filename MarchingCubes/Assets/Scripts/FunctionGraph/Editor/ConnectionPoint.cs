using System;
using UnityEngine;

public class ConnectionPoint
{
    public enum ConnectionPointType 
    {
        Out,
        InSingle,
        InMultiple
    }

    public ConnectionPointType PointType { get; protected set; }

    GUIStyle style;

    FunctionGraphEditorNode nodeBelongingTo;
    Action<FunctionGraphEditorNode, int> OnConnectionPointClick;
    Rect offsetRect;
    int idx;

    public ConnectionPoint(ConnectionPointType pointType,int idx, FunctionGraphEditorNode nodeBelongingTo,Rect offsetRect, GUIStyle style, Action<FunctionGraphEditorNode,int> onConnectionPointClick)
    {
        PointType = pointType;
        this.idx = idx;
        this.offsetRect = offsetRect;

        this.nodeBelongingTo = nodeBelongingTo ?? throw new ArgumentNullException(nameof(nodeBelongingTo));
        OnConnectionPointClick = onConnectionPointClick ?? throw new ArgumentNullException(nameof(onConnectionPointClick));
        this.style = style;
    }

    public void Draw()
    {
        Rect r = CalculateDrawRect();

        if (GUI.Button(r, "", style))
        {
            OnConnectionPointClick?.Invoke(nodeBelongingTo,idx);
            if (PointType == ConnectionPointType.InMultiple)
                idx++;
        }
    }

    private Rect CalculateDrawRect()
    {
        Rect r = offsetRect;
        r.y += nodeBelongingTo.Rect.y;
        r.x += nodeBelongingTo.Rect.x;
        return r;
    }

    public void UnsetNodeBelongingTo()
    {
        nodeBelongingTo = null;
    }

    public bool ProcessEvent(Event e)
    {
        //throw new NotImplementedException();
        return false;
    }
}
