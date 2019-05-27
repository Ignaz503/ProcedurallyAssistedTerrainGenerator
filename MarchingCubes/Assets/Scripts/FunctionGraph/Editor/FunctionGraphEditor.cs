﻿using System.Collections;
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
    List<FunctionGraphEditorNode> nodesList;
    List<ConnectionToDraw> connectionsToDraw;
    
    public void Initialize()
    {
        graph = new FunctionGraph();
        connectionsToDraw = new List<ConnectionToDraw>();
        
        nodesList = new List<FunctionGraphEditorNode>();

        nodes = new Dictionary<BaseFuncGraphNode, FunctionGraphEditorNode>();
        LoadSettings();
    }

    private void LoadSettings()
    {
    }

    public void OnGUI()
    {
        //clear old connections
        connectionsToDraw.Clear();
        //DO STUFF

        DrawNodes();
        DrawConnections();

        ProcessNodeEvents(Event.current);
        ProcessEvent(Event.current);
        if (GUI.changed) Repaint();
    }

    private void ProcessEvent(Event e)
    {
        throw new NotImplementedException();
    }

    private void ProcessNodeEvents(Event e)
    {
        if (nodes != null)
        {
            for (int i = nodesList.Count-1 ; i  >= 0; i--)
            {
                bool guiChanged = nodesList[i].ProcessEvent(e);
                if (guiChanged)//don't set directly to guiChanged cause we don't wanna set it flase
                    GUI.changed = true;
            }
        }
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
        nodesList.Add(node);
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
        nodesList.Remove(functionGraphEditorNode);
    }
}
