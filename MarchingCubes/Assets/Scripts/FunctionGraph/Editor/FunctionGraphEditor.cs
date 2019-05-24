using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FunctionGraphEditor : EditorWindow
{
    [MenuItem("Window/Function Graph Editor")]
    private static void OpenWindow()
    {
        FunctionGraphEditor window = GetWindow<FunctionGraphEditor>();

        window.titleContent = new GUIContent("Function Graph Editor");
        window.Initialize();
    }

    FunctionGraph graph;

    public Dictionary<BaseFuncGraphNode, FunctionGraphEditorNode> nodes;
    List<ConnectionToDraw> connectionsToDraw;


    public void Initialize()
    {
        graph = new FunctionGraph();
        connectionsToDraw = new List<ConnectionToDraw>();
        nodes = new Dictionary<BaseFuncGraphNode, FunctionGraphEditorNode>();
    }

    public void OnGUI()
    {
        //clear old connections
        connectionsToDraw.Clear();
        //DO STUFF

        DrawNodes();
    }

    void DrawNodes()
    {
        foreach (var node in nodes.Values)
        {
            node.Draw();
        }
    }

    void DrawConnections()
    {
        for (int i = 0; i < connectionsToDraw.Count; i++)
        {
            connectionsToDraw[i].Draw();
        }
    }

    public void AddNode(FunctionGraphEditorNode node)
    {
        nodes.Add(node.Node, node);
    }

    public FunctionGraphEditorNode GetNode(BaseFuncGraphNode n)
    {
        if (nodes.ContainsKey(n))
            return nodes[n];
        return null;
    }

    public void AddConnectionToDraw(ConnectionToDraw toDraw)
    {
        connectionsToDraw.Add(toDraw);
    }
}

public struct ConnectionToDraw
{
    public Vector2 from;
    public Vector2 to;

    public FunctionGraphEditorNode fromNode;
    public FunctionGraphEditorNode toNode;

    public void Draw()
    {
        //TODO Implement
    } 
}