using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public class FunctionGraphEditorNode
{
    public event Action<FunctionGraphEditorNode, ConnectionPoint, int> OnInConnectionPointClicked;
    public event Action<FunctionGraphEditorNode, ConnectionPoint, int> OnOutConnectionPointClicked;

    public BaseFuncGraphNode GraphNode { get; protected set; }
    FunctionGraphEditor editorBelongingTo;
    public FunctionGraphEditor Editor { get { return editorBelongingTo; } }

    Rect rect;
    public Rect Rect
    {
        get
        {
            return rect;
        }
        set
        {
            rect = value;
        }
    }

    ConnectionToDraw conToDraw;
    protected GUIStyle style;

    public bool isDragged;

    List<ConnectionPoint> connectionPoints;

    public FunctionGraphEditorNode(Vector2 creationPosition, BaseFuncGraphNode node, FunctionGraphEditor editor, FunctionGraphEditorNodeLayout layout)
    {
        editorBelongingTo = editor;
        GraphNode = node;
        connectionPoints = new List<ConnectionPoint>();
        CreateNodeDrawableInfo(creationPosition, layout);
    }

    public void Draw()
    {
        //Debug.Log($"Draw Node {GraphNode.ShortDescription}");
        //todo: draw rect
        DrawNode();


        //draw connection points
        for (int i = 0; i < connectionPoints.Count; i++)
        {
            connectionPoints[i].Draw();
        }

        if (GraphNode.HasParent)
        {
            //wehn we set the parent we need to update the conn to draw object
            //todo: Draw connection and so on
            editorBelongingTo.AddConnectionToDraw(conToDraw);
        }
    }

    protected virtual void DrawNode()
    {
        GUI.Box(Rect, GraphNode.ShortDescription, style);
    }

    public void CreateConnection(FunctionGraphEditorNode to, int idx, ConnectionPoint fromPoint, ConnectionPoint toPoint)
    {
        // set parent
        GraphNode.ParentTo(to.GraphNode, idx);

        //create con ToDraw
        CreateConnectionDrawable(to, fromPoint, toPoint);
    }


    public ConnectionPoint GetConnectionPoint(int idx)
    {
        return connectionPoints[idx];
    }

    private void CreateConnectionDrawable(FunctionGraphEditorNode to, ConnectionPoint fromPoint, ConnectionPoint toPoint)
    {
        conToDraw = new ConnectionToDraw(this, to, fromPoint, toPoint, to.connectionPoints.IndexOf(toPoint));
    }

    private void RemoveConnectionToDrawDrawable()
    {
        if (conToDraw != null)
        {
            editorBelongingTo.RemoveConnection(conToDraw);
            conToDraw = null;
        }
    }

    public void DeleteNode()
    {
        if (GraphNode.HasParent)
        {
            GraphNode.UncoupleFromParent();
            editorBelongingTo.RemoveConnection(conToDraw);
            conToDraw = null;
        }

        if (GraphNode is ParentableNode)
        {
            var n = GraphNode as ParentableNode;

            //unset con to draw for all child nodes
            foreach (BaseFuncGraphNode node in n)
            {
                if (node != null)
                {
                    var editorNode = editorBelongingTo.GetNode(node);
                    editorNode.RemoveConnectionToDrawDrawable();
                }
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
        GraphNode = null;
    }

    public bool ProcessEvent(Event e)
    {
        if (Rect.Contains(e.mousePosition))
        {
            return ProvessEventForNode(e);
        }
        return false;
    }

    protected virtual bool ProvessEventForNode(Event e)
    {
        //we know there is a button click
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 1)
                {
                    e.Use();
                    ShowNodeGenericMenu();
                }
                if (e.button == 0)
                {
                    //start drag
                    isDragged = true;
                    GUI.changed = true;
                }
                break;
            case EventType.MouseUp:
                isDragged = false;
                break;
            case EventType.MouseDrag:
                if (e.button == 0 && isDragged)
                {
                    Drag(e.delta);
                    e.Use();
                    return true;
                }
                break;
        }
        return false;
    }

    public void Drag(Vector2 delta)
    {
        rect.position += delta;
    }

    private void ShowNodeGenericMenu()
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("Remove"), false, () => DeleteNode());
        menu.AddItem(new GUIContent("Set As Evaluation Start Point"), false, () => MakeNodeEvaluationStartPoint());

        menu.ShowAsContext();
    }

    void MakeNodeEvaluationStartPoint()
    {
        Debug.Log("Marking Node as root");
        editorBelongingTo.MarkeAsRoot(GraphNode);
    }

    public void DeleteConnection()
    {
        if (GraphNode.HasParent)
        {
            GraphNode.UnsetParent();
            RemoveConnectionToDrawDrawable();
            editorBelongingTo.NotifyOfGraphValidityChange();
        }
    }

    public virtual void CreateNodeDrawableInfo(Vector2 creationPosition, FunctionGraphEditorNodeLayout layout)
    {
        //one out conn point
        Rect = new Rect(creationPosition, new Vector2(layout.Width, layout.Height));
        style = layout.Style;

        //create connection points
        CreateConnectionPoints(layout);
    }

    private void CreateConnectionPoints(FunctionGraphEditorNodeLayout layout)
    {
        CreateConnectionPoints(layout.Width, layout.Height, layout, FunctionGraphEditorNodeLayout.ListType.Out, OnOutConnectionPointClick);

        CreateConnectionPoints(layout.Width, layout.Height, layout, FunctionGraphEditorNodeLayout.ListType.In, OnInConnectionPointClick);
    }

    private void CreateConnectionPoints(float width, float height, FunctionGraphEditorNodeLayout layout, FunctionGraphEditorNodeLayout.ListType list, Action<ConnectionPoint, int> onClick)
    {
        int count = (list == 0) ? layout.InConnectionPointCount : layout.OutConnectionPointCount;
        //loop over all set create type and set offset rect correctly
        for (int i = 0; i < count; i++)
        {
            FunctionGraphEditorNodeLayout.ConnectionPointInfo info = layout[list, i];

            //figure out width and height
            Vector2 size = new Vector2(width * info.Width, height * info.Height);

            //figure out offset
            Vector2 offset = new Vector2(width, height);
            offset.Scale(new Vector2(info.UVx, info.UVy));
            offset -= size * .5f;

            connectionPoints.Add(
                new ConnectionPoint(
                    info.Type,
                    i,
                    this,
                    new Rect(offset, size),
                    info.Style,
                    onClick
                    ));

        }

    }

    void OnOutConnectionPointClick(ConnectionPoint conP, int nodeChildIdx)
    {
        //TODO: realay event
        OnOutConnectionPointClicked?.Invoke(this, conP, nodeChildIdx);
    }

    void OnInConnectionPointClick(ConnectionPoint conP, int nodeChildIndex)
    {
        //TODO realay event
        OnInConnectionPointClicked?.Invoke(this, conP, nodeChildIndex);
    }

    public virtual FunctionGraphEditorNodeSerializable CreateSerializeable()
    {
        FunctionGraphEditorNodeSerializable ser = new FunctionGraphEditorNodeSerializable();

        ser.EditorNodeType = GetType().ToString();
        ser.NodePosition = Rect.position;
        ser.NodeValue = "";
        ser.GraphNodeType = GraphNode.GetType().ToString();
        

        if (GraphNode.HasParent)
        {
            var parent = Editor.GetNode(GraphNode.Parent);
            ser.ParentIndex = Editor.GetListIndexOf(parent);
            ser.ToIdx = conToDraw.EditorNodeConnectionPointIndex;
            ser.FromIdx = connectionPoints.IndexOf(conToDraw.fromPoint);
        }
        else
        {
            ser.ParentIndex = -1;
            ser.ToIdx = ser.FromIdx = -1;
            ser.FromIdx = -1;
        }

        return ser;
    }

    public virtual void DeserializeData(string data)
    {
        return;
    }

    public void InformOfConnectionToDraw()
    {
        editorBelongingTo.AddConnectionToDraw(conToDraw);
        GUI.changed = true;
    }

}
