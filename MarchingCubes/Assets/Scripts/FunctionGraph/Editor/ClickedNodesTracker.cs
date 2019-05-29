using System;

public struct ClickedNodesTracker
{
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
    }
}

public struct ClickedNodeInfo
{
    public FunctionGraphEditorNode Node;
    public ConnectionPoint ConPoint;
    public int NodeChildIdx;

    public ClickedNodeInfo(FunctionGraphEditorNode node, ConnectionPoint conPoint, int nodeChildIdx)
    {
        Node = node ?? throw new ArgumentNullException(nameof(node));
        ConPoint = conPoint ?? throw new ArgumentNullException(nameof(conPoint));
        NodeChildIdx = nodeChildIdx;
    }
}


