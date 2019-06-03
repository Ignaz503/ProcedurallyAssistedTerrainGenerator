using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;

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
    ILogger editorLogger;

    private Vector2 offset;
    private Vector2 drag;
    bool isValidGraph = false;

    public void Initialize()
    {

        graph = new FunctionGraph();
        connectionsToDraw = new List<ConnectionToDraw>();
        isValidGraph = false;
        
        nodesList = new List<FunctionGraphEditorNode>();
        clickedNodeTraker = new ClickedNodesTracker();
        clickedNodeTraker.Editor = this; 

        nodes = new Dictionary<BaseFuncGraphNode, FunctionGraphEditorNode>();
        editorLogger = Debug.unityLogger;
        FunctionGraphValidationMessageLogger.Mode = FunctionGraphValidationMessageLogger.LoggerMode.NotificationNoCollect;
        //LoadSettings();
    }

    //private void LoadSettings()
    //{
    //}

    public void OnGUI()
    {
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        //clear old connections
        connectionsToDraw.Clear();
        //DO STUFF

        DrawNodes();
        DrawConnections();
        clickedNodeTraker.DrawDummyConnection(Event.current.mousePosition);

        ProcessNodeEvents(Event.current);
        bool changed = ProcessEvent(Event.current);

        //Just draw the editor buttons
        DrawEditorButtons();

        if (GUI.changed || changed) Repaint();
    }

    public void MarkeAsRoot(BaseFuncGraphNode node)
    {
        graph.RootNode = node;
    }

    private void DrawEditorButtons()
    {

        //figure out width and height
        Vector2 size = new Vector2(1f * position.size.x, .15f * position.size.y);

        //figure out offset
        Vector2 offset = position.size;
        offset.Scale(new Vector2(0f, .95f));
       // offset -= size * .5f;

        GUILayout.BeginArea(new Rect(offset,size));
        GUILayout.BeginHorizontal();
        GUILayout.Label("Graph Name: ",EditorStyles.boldLabel);
        graph.GraphName = GUILayout.TextField(graph.GraphName);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        
        if(GUILayout.Button("Validate"))
        {
            ValidateGraph();

        }

        if (GUILayout.Button("Dummy Evaluate (x:0,y:0,z:0)"))
        {
            Log(graph.Evaluate(new SamplePointVariables(0,0), new SamplePointVariables(0, 0), new SamplePointVariables(0, 0)).ToString());
        }

        GUILayout.EndHorizontal();
        DrawSaveAndLoadButtons();
        GUILayout.EndArea();
    }

    void ValidateGraph()
    {
        if (graph.ValidateGraph(editorLogger) == 0)
        {
            Log("Valid Graph");
            isValidGraph = true;
        }
        else
        {
            LogWarning("Warning", "Graph is not valid");
            isValidGraph = false;
        }
    }

    private void DrawSaveAndLoadButtons()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Load"))
        {
            string path = EditorUtility.OpenFilePanel("Graph To open", Application.dataPath, "asset");

            if (path.Length != 0)
            {
                Load(path);
            }
        }


        if (graph.GraphName == null || graph.GraphName == "")
            return;
        if (GUILayout.Button("Compile To C#"))
        {
            ValidateGraph();
            if(!isValidGraph)
                return;

            string path = EditorUtility.SaveFilePanelInProject("Compile Graph", graph.GraphName, "cs",
                "Please enter a file name to save the compiled graph to");

            if (path.Length != 0)
            {
                using (var writer = new StreamWriter(path))
                {
                    graph.Write(writer);
                }
            }
            AssetDatabase.Refresh();
        }

        if (GUILayout.Button("Save"))
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Graph", graph.GraphName, "asset",
    "Please enter a file name to save the graph to");

            if (path.Length != 0)
            {
                Save(path);
            }
        }


        GUILayout.EndHorizontal();
    }

    private bool ProcessEvent(Event e)
    {
        drag = Vector2.zero;

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
            case EventType.MouseDrag:
                if (e.button == 0)
                {
                    OnDrag(e.delta);
                    
                }
                break;
            default:
                break;
        }
        return false;
    }

    private void OnDrag(Vector2 delta)
    {
        drag = delta;

        for (int i = 0; i < nodesList.Count; i++)
        {
            nodesList[i].Drag(delta);
        }

        GUI.changed = true;
    }

    private void CreateNode(Vector2 mousePosition, BaseFuncGraphNode n)
    {
        var layout = settings.GetLayout(n);
        Vector2 pos = mousePosition - layout.Size*.5f;

        AddNode(FunctionGraphEditorNodeFactory.CreateEditorNode(mousePosition, n, this, layout));

    }

    /// <summary>
    /// http://gram.gs/gramlog/creating-node-based-editor-unity/
    /// </summary>
    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    public void NotifyOfGraphValidityChange()
    {
        isValidGraph = false;
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
        isValidGraph = false;
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
        editorLogger.LogError(tag, msg);
    }

    public void LogWarning(string tag, string msg)
    {
        editorLogger.LogWarning(tag, msg);
    }

    public void Log(string msg)
    {
        editorLogger.Log(msg);
    }

    public int GetIndex(FunctionGraphEditorNode n)
    {
        return nodesList.IndexOf(n);
    }

    public void Save(string path)
    {
        var data = CreateInstance<FunctionGraphEditorData>();
        data.Nodes = new List<FunctionGraphEditorNodeSerializable>();
        data.GraphName = graph.GraphName;
        for (int i = 0; i < nodesList.Count; i++)
        {
            if (!nodesList[i].GraphNode.HasParent)
            {
                // write all nodes without a parent
                // other nodes taken care of by parent
                data.Nodes.Add(nodesList[i].CreateSerializeable());
            }
        }
        data.Save(path);
    }

    public void Load(string path)
    {
        string s = MakeRelativePath(path);

        var data = AssetDatabase.LoadAssetAtPath<FunctionGraphEditorData>(s);

        if (data == null)
        {
            Debug.LogError("Failed to load graph at " + s);
            return;
        }

        graph = new FunctionGraph();
        for (int i = nodesList.Count -1 ; i >= 0; i--)
        {
            nodesList[i].DeleteNode();
        }

        nodes.Clear();
        nodes.Clear();
        connectionsToDraw.Clear();

        graph.GraphName = data.GraphName;
        
        foreach (var nodeData in data.Nodes)
        {
            DeserializeNodeData(nodeData);
        }
    }

    /// <summary>
    /// FUKCING RETARDED WAY TO MAKE RELATIVE PATH FROM ABSOLUTE
    /// </summary>
    private string MakeRelativePath(string path)
    {
        return path.Replace(Application.dataPath.Substring(0,Application.dataPath.LastIndexOf('/') + 1), "");
    }

    private void DeserializeNodeData(FunctionGraphEditorNodeSerializable nodeData)
    {
        BaseFuncGraphNode n = FuncGraphNodeFactory.CreateNode(Type.GetType($"{nodeData.GraphNodeType}, {Assembly.GetAssembly(typeof(BaseFuncGraphNode)).FullName}"), graph);
        CreateNode(nodeData.NodePosition, n);

        var nodeParent = nodesList[nodesList.Count - 1];//last elem

        for (int i = 0; i < nodeData.Children.Count; i++)
        {
            n = FuncGraphNodeFactory.CreateNode(Type.GetType($"{nodeData.Children[i].GraphNodeType}, {Assembly.GetAssembly(typeof(BaseFuncGraphNode)).FullName}"), graph);
            CreateNode(nodeData.Children[i].NodePosition, n);

            var recentChild = nodesList[nodesList.Count - 1];

            if (nodeData.Children[i].fromIDX >= 0 && nodeData.Children[i].toIdx >= 0)
            {
                recentChild.CreateConnection(nodeParent, nodeData.toIdx, recentChild.GetConnectionPoint(nodeData.Children[i].fromIDX), nodeParent.GetConnectionPoint(nodeData.Children[i].toIdx));
            }
            recentChild.DeserializeData(nodeData.Children[i].NodeValue);
        }
        Repaint();
    }
}

[Serializable]
public class FunctionGraphEditorData : ScriptableObject
{
    public string GraphName;
    public List<FunctionGraphEditorNodeSerializable> Nodes;

    public void Save(string path)
    {
        AssetDatabase.CreateAsset(this, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}