using System;
using UnityEngine;

[Serializable]
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
    string label;

    FunctionGraphEditorNode nodeBelongingTo;
    Action<ConnectionPoint,int> OnConnectionPointClick;
    public Rect OffsetRect { get; protected set; }
    int idx;
    public int Idx { get { return idx; } }

    public ConnectionPoint(ConnectionPointType pointType,int idx, FunctionGraphEditorNode nodeBelongingTo,Rect offsetRect, GUIStyle style, Action<ConnectionPoint, int> onConnectionPointClick, string label = "")
    {
        PointType = pointType;
        this.idx = idx;
        this.OffsetRect = offsetRect;
        this.label = label;

        this.nodeBelongingTo = nodeBelongingTo ?? throw new ArgumentNullException(nameof(nodeBelongingTo));
        OnConnectionPointClick = onConnectionPointClick ?? throw new ArgumentNullException(nameof(onConnectionPointClick));
        this.style = style;
    }

    public void Draw()
    {
        Rect r = CalculateDrawRect();

        if (GUI.Button(r, label, style))
        {
            OnConnectionPointClick?.Invoke(this,idx);
            if (PointType == ConnectionPointType.InMultiple)
                idx++;
        }
    }

    private Rect CalculateDrawRect()
    {
        Rect r = OffsetRect;
        r.y += nodeBelongingTo.Rect.y;
        r.x += nodeBelongingTo.Rect.x;
        return r;
    }

    public void UnsetNodeBelongingTo()
    {
        nodeBelongingTo = null;
    }
    
}
