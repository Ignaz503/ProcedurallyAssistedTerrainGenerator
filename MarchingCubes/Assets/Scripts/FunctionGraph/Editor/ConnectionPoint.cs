﻿using System;
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
    Action<FunctionGraphEditorNode> OnConnectionPointClick;
    Rect offsetRect;

    public ConnectionPoint(ConnectionPointType pointType, FunctionGraphEditorNode nodeBelongingTo,Rect offsetRect, GUIStyle style, Action<FunctionGraphEditorNode> onConnectionPointClick)
    {
        PointType = pointType;

        this.offsetRect = offsetRect;

        this.nodeBelongingTo = nodeBelongingTo ?? throw new ArgumentNullException(nameof(nodeBelongingTo));
        OnConnectionPointClick = onConnectionPointClick ?? throw new ArgumentNullException(nameof(onConnectionPointClick));
    }

    public void Draw()
    {
        Rect r = CalculateDrawRect();

        if (GUI.Button(r, "", style))
        {
            OnConnectionPointClick?.Invoke(nodeBelongingTo);
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
}