using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionGraphEditorNode
{
    public BaseFuncGraphNode Node { get; protected set; }
    FunctionGraphEditor editorBelongingTo;
    public FunctionGraphEditor Editor { get { return editorBelongingTo; } }
    public Rect Rect { get; protected set; }
    ConnectionToDraw conToDraw;
    GUIStyle style;

    List<ConnectionPoint> connectionPoints;

    public FunctionGraphEditorNode(Vector2 creationPosition, BaseFuncGraphNode node, FunctionGraphEditor editor)
    {
        editorBelongingTo = editor;
        Node = node;
        connectionPoints = new List<ConnectionPoint>();
        CreateNodeDrawable(creationPosition);
    }
    
    public virtual void Draw()
    {
        //todo: draw rect
        GUI.Box(Rect, Node.ShortDescription,style);

        //draw connection points
        for (int i = 0; i < connectionPoints.Count; i++)
        {
            connectionPoints[i].Draw();
        }

        if (Node.HasParent)
        { 
            //wehn we set the parent we need to update the conn to draw object
            //todo: Draw connection and so on
            editorBelongingTo.AddConnectionToDraw(conToDraw);     
        }
    }

    public virtual void HandleEvent(Event e)
    {
        //TODO event handling
        //create connection
        //connections point handling
    }

    public void CreateConnection(FunctionGraphEditorNode to, int idx)
    {
        // set parent
        Node.ParentTo(to.Node, idx);

        //create con ToDraw
        CreateConnectionDrawable();
    }

    private void CreateConnectionDrawable()
    {
        
    }

    private void RemoveConnectionToDrawDrawable()
    {
        if(conToDraw != null)
        {
            editorBelongingTo.RemoveConnection(conToDraw);
            conToDraw = null;
        }
    }

    public void DeleteNode()
    {
        //
        if (Node.HasParent)
        {
            Node.UncoupleFromParent();
            editorBelongingTo.RemoveConnection(conToDraw);
            conToDraw = null;
        }
        if(Node is ParentableNode)
        {
            var n = Node as ParentableNode;

            //unset con to draw for all child nodes
            foreach (BaseFuncGraphNode node  in n)
            {
                var editorNode = editorBelongingTo.GetNode(node);
                editorNode.RemoveConnectionToDrawDrawable();
            }

            //make sure to unset all children
            n.RemoveAllChildren();
        }
        //probably not necessary
        // but maybe avoid circular reference and no garbage collection
        for (int i = 0; i < connectionPoints.Count; i++)
        {
            connectionPoints[i].UnsetNodeBelongingTo();
        }
        //inform graph
        editorBelongingTo.RemoveNode(this);
        Node = null;
    }
 
    public void DeleteConnection()
    {
        if (Node.HasParent)
        {
            Node.UnsetParent();
            RemoveConnectionToDrawDrawable();
        }
    }

    public virtual void CreateNodeDrawable(Vector2 creationPosition)
    {
        //one out conn point
        FuncGraphEditorNodeSettings settings = editorBelongingTo.GetSettingsForNode(Node);
        style = settings.Style;
        Rect = new Rect(creationPosition, new Vector2(settings.Width,settings.Height));

        //create out conneciton
        CreateOutPoint();


        //TODO Place in points
        CreatInPoints();
    }

    private void CreatInPoints()
    {
        //TODO
    }

    void CreateOutPoint()
    {
        //middle top
        FuncGraphEditorConnectionPointSettings outPointSettings = editorBelongingTo.GetSettingsForConnectionPoint(ConnectionPoint.ConnectionPointType.Out, Node);

        Rect outPointOffsetRect = new Rect(0, 0, outPointSettings.Width, outPointSettings.Height);

        outPointOffsetRect.y = (Rect.height * .5f) - outPointSettings.Height * .5f;
        outPointOffsetRect.x = (Rect.width * .5f) - outPointSettings.Width * .5f;

        connectionPoints.Add(new ConnectionPoint(ConnectionPoint.ConnectionPointType.Out, this, outPointOffsetRect, outPointSettings.Style, OnOutConnectionPointClick));
    }

    void OnOutConnectionPointClick(FunctionGraphEditorNode n)
    {
        //TODO 
    }

    void OnInConnectionPointClick(FunctionGraphEditorNode n)
    {
        //TODO
    }
}

public class FuncGraphEditorNodeSettings
{
    public GUIStyle Style;
    public float Width;
    public float Height;
    public FuncGraphEditorConnectionPointSettings OutSetting;
    public FuncGraphEditorConnectionPointSettings InSetting;
}

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

[CreateAssetMenu(menuName = "Connection Point Setting")]
public class FuncGraphEditorConnectionPointSettings : ScriptableObject
{
    public GUIStyle Style = null;
    public float Height = 50;
    public float Width = 50;
}