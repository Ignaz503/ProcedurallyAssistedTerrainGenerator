using System;
using UnityEngine;

public class FunctionGraphEditorNode
{
    public BaseFuncGraphNode Node { get; protected set; }
    FunctionGraphEditor editorBelongingTo;
    Rect rect;
    ConnectionToDraw conToDraw;

    public FunctionGraphEditorNode(BaseFuncGraphNode node, FunctionGraphEditor editor)
    {
        editorBelongingTo = editor;
        Node = node;
        //editor.AddNode(this); maybe here maybe not here
    }
    
    public virtual void Draw()
    {
        //todo: draw rect
        
        //draw connection points

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
            Node.UnsetParent();
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

}

