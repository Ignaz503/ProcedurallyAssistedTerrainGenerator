using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public partial class FunctionGraphEditor : EditorWindow
{
    [MenuItem("Window/Function Graph Editor")]
    private static void OpenWindow()
    {
        FunctionGraphEditor window = GetWindow<FunctionGraphEditor>();

        window.titleContent = new GUIContent("Function Graph Editor");
        window.Initialize();
    }

    FunctionGraph graph;

    [SerializeField] FunctionGraphEditorSettings settings = null;
    public Dictionary<BaseFuncGraphNode, FunctionGraphEditorNode> nodes;
    List<FunctionGraphEditorNode> nodesList;
    List<ConnectionToDraw> connectionsToDraw;
    ClickedNodesTracker clickedNodeTraker;
    ILogger editorLoger;
    
    public void Initialize()
    {
        graph = new FunctionGraph();
        connectionsToDraw = new List<ConnectionToDraw>();
        
        nodesList = new List<FunctionGraphEditorNode>();
        clickedNodeTraker = new ClickedNodesTracker();
        clickedNodeTraker.Editor = this; 

        nodes = new Dictionary<BaseFuncGraphNode, FunctionGraphEditorNode>();
        editorLoger = Debug.unityLogger;
        //LoadSettings();
    }

    //private void LoadSettings()
    //{
    //}

    public void OnGUI()
    {
        //clear old connections
        connectionsToDraw.Clear();
        //DO STUFF

        DrawNodes();
        DrawConnections();
        clickedNodeTraker.DrawDummyConnection(Event.current.mousePosition);

        ProcessNodeEvents(Event.current);
        bool changed = ProcessEvent(Event.current);
        if (GUI.changed || changed) Repaint();
    }

    private bool ProcessEvent(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 1)
                {
                    if (clickedNodeTraker.IsTracking && e.modifiers == EventModifiers.Control)
                        clickedNodeTraker.Reset();
                    else
                        ProcessRightClickContextMenu(e);
                    return true;
                }
                break;
            default:
                break;
        }
        return false;
    }

    private void CreateNode(Vector2 mousePosition, BaseFuncGraphNode n)
    {
        var layout = settings.GetLayout(n);
        Vector2 pos = mousePosition - layout.Size*.5f;
        if(!(n is ConstantNode) && !(n is VariableNode))
            AddNode(new FunctionGraphEditorNode(pos, n, this, layout));
        else if(n is ConstantNode)
            AddNode(new FunctionGraphEditorNodeConstant(pos, n, this, layout));
        else
            AddNode(new FunctionGraphEditorNodeVariable(pos, n, this, layout));
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
        nodes.Add(node.GraphNode, node);
        nodesList.Add(node);
        node.OnInConnectionPointClicked += OnInConnectionPointClicked;
        node.OnOutConnectionPointClicked += OnOutConnectionPointClicked;
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
        nodes.Remove(functionGraphEditorNode.GraphNode);
        nodesList.Remove(functionGraphEditorNode);
    }

    public void ProcessRightClickContextMenu(Event e)
    {
        GenericMenu genericMenu = new GenericMenu();
        //genericMenu.AddItem(new GUIContent("Add Node"), false, () => OnClickAddNode(e.mousePosition));
        foreach (var type in BaseFuncGraphNode.InstantiableNodeTypes)
        {
            var node = FuncGraphNodeFactory.CreateNode(type, graph);
            genericMenu.AddItem(new GUIContent($"{node.ShortDescription}"), false, () => CreateNode(e.mousePosition, node));
        }

        genericMenu.ShowAsContext();
    }

    public void OnOutConnectionPointClicked(FunctionGraphEditorNode node, ConnectionPoint point, int nodeChildIdx)
    {

        clickedNodeTraker.SetOutNode(node, point, nodeChildIdx);

    }

    public void OnInConnectionPointClicked(FunctionGraphEditorNode node, ConnectionPoint point, int nodeChildIdx)
    {
        //TODO

        clickedNodeTraker.SetInNode(node, point, nodeChildIdx);
    }

    public void LogError(string tag, string msg)
    {
        editorLoger.LogError(tag, msg);
    }

    public void LogWarning(string tag, string msg)
    {
        editorLoger.LogWarning(tag, msg);
    }

    public void Log(string msg)
    {
        editorLoger.Log(msg);
    }

}
