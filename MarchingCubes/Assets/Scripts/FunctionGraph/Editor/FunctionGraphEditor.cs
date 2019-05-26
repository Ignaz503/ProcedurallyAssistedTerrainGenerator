using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

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

    [SerializeField]FuncGraphEditorNodeSettings defaultNodeSetting;
    Dictionary<Type, FuncGraphEditorNodeSettings> nodeSettings;
    
    public void Initialize()
    {
        graph = new FunctionGraph();
        connectionsToDraw = new List<ConnectionToDraw>();

        nodeSettings = new Dictionary<Type, FuncGraphEditorNodeSettings>();

        nodes = new Dictionary<BaseFuncGraphNode, FunctionGraphEditorNode>();
        LoadSettings();
    }

    private void LoadSettings()
    {
        //TODO
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

    public void RemoveConnection(ConnectionToDraw conToDraw)
    {
        connectionsToDraw.Remove(conToDraw);
    }

    public void AddConnectionToDraw(ConnectionToDraw toDraw)
    {
        if (toDraw == null)
            return;
        connectionsToDraw.Add(toDraw);
    }

    public void RemoveNode(FunctionGraphEditorNode functionGraphEditorNode)
    {
        nodes.Remove(functionGraphEditorNode.Node);
    }

    public FuncGraphEditorConnectionPointSettings GetSettingsForConnectionPoint(ConnectionPoint.ConnectionPointType pointType, BaseFuncGraphNode node)
    {
        switch (pointType)
        {
            case ConnectionPoint.ConnectionPointType.InMultiple:
                if (nodeSettings.ContainsKey(node.GetType()))
                {
                    return nodeSettings[node.GetType()].InSetting;
                }
                return defaultNodeSetting.InSetting;
            case ConnectionPoint.ConnectionPointType.InSingle:
                if (nodeSettings.ContainsKey(node.GetType()))
                {
                    return nodeSettings[node.GetType()].InSetting;
                }
                return defaultNodeSetting.InSetting;
            case ConnectionPoint.ConnectionPointType.Out:
                if (nodeSettings.ContainsKey(node.GetType()))
                {
                    return nodeSettings[node.GetType()].OutSetting;
                }
                return defaultNodeSetting.OutSetting;
        }
        return null;
    }

    internal FuncGraphEditorNodeSettings GetSettingsForNode(BaseFuncGraphNode node)
    {
        if (nodeSettings.ContainsKey(node.GetType()))
        {
            return nodeSettings[node.GetType()];
        }
        return defaultNodeSetting;
    }
}

public class ConnectionToDraw
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