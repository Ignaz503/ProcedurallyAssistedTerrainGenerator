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
}

