using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionGraphEditorNode
{
    public event Action<FunctionGraphEditorNode, int> OnInConnectionPointClicked;
    public event Action<FunctionGraphEditorNode, int> OnOutConnectionPointClicked;
    
    public BaseFuncGraphNode Node { get; protected set; }
    FunctionGraphEditor editorBelongingTo;
    public FunctionGraphEditor Editor { get { return editorBelongingTo; } }
    public Rect Rect { get; protected set; }
    ConnectionToDraw conToDraw;
    GUIStyle style;

    List<ConnectionPoint> connectionPoints;

    public FunctionGraphEditorNode(Vector2 creationPosition, BaseFuncGraphNode node, FunctionGraphEditor editor,FunctionGraphEditorNodeLayout layout)
    {
        editorBelongingTo = editor;
        Node = node;
        connectionPoints = new List<ConnectionPoint>();
        CreateNodeDrawable(creationPosition,layout);
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

    public  bool ProcessEvent(Event e)
    {
        bool change = false;
        for (int i = 0; i < connectionPoints.Count; i++)
        {
            bool ret = connectionPoints[i].ProcessEvent(e);
            if (ret)//don't want to set to false
                change = ret;
        }
        return change || ProvessEventForNode(e);
    }

    protected virtual bool ProvessEventForNode(Event e)
    {
        return false;
    }

    public void DeleteConnection()
    {
        if (Node.HasParent)
        {
            Node.UnsetParent();
            RemoveConnectionToDrawDrawable();
        }
    }

    public virtual void CreateNodeDrawable(Vector2 creationPosition,FunctionGraphEditorNodeLayout layout)
    {
        //one out conn point
        Rect = new Rect(creationPosition, new Vector2(layout.Width, layout.Height));
        style = layout.Style;

        //create connection points
        CreateConnectionPoints(layout);
    }

    private void CreateConnectionPoints(FunctionGraphEditorNodeLayout layout)
    {
        CreateConnectionPoints(layout.Width, layout.Height, layout, FunctionGraphEditorNodeLayout.ListType.In, OnInConnectionPointClick);

        CreateConnectionPoints(layout.Width, layout.Height, layout , FunctionGraphEditorNodeLayout.ListType.Out, OnOutConnectionPointClick);
    }

    private void CreateConnectionPoints(float width, float height, FunctionGraphEditorNodeLayout layout,FunctionGraphEditorNodeLayout.ListType list, Action<int> onClick)
    {
        int count = (list == 0) ? layout.InConnectionPointCount : layout.OutConnectionPointCount;
        //loop over all set create type and set offset rect correctly
        for (int i = 0; i < count; i++)
        {
            FunctionGraphEditorNodeLayout.ConnectionPointInfo info = layout[list,i];

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

    void OnOutConnectionPointClick(int idx)
    {
        //TODO 
        Debug.Log($"Out Con Point {idx} Clicked");
        //TODO: realay event
        OnOutConnectionPointClicked?.Invoke(this, idx);
    }

    void OnInConnectionPointClick(int idx)
    {
        //TODO
        Debug.Log($"In Con Point {idx} Clicked");
        //TODO realay event
        OnInConnectionPointClicked?.Invoke(this, idx);
    }

}
