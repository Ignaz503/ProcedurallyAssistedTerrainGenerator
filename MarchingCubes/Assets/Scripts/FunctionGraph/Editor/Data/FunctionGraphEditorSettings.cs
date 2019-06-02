using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Function Graph Editor/ Editor Settings")]
public class FunctionGraphEditorSettings : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] FunctionGraphEditorNodeLayout defaultNoIn = null;

    [SerializeField] FunctionGraphEditorNodeLayout defaultSingleIn = null;
    [SerializeField] FunctionGraphEditorNodeLayout defaultDualIn = null;
    [SerializeField] FunctionGraphEditorNodeLayout defaultMultiIn = null;
    [SerializeField] FunctionGraphEditorNodeLayout defaultTripleIn = null;

    Dictionary<Type, FunctionGraphEditorNodeLayout> layoutMapping = null;

    [HideInInspector] [SerializeField] List<NodeTypeToNodeLayout> serializableMapping;

    private void Awake()
    {
        CreateMapping();     
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(FunctionGraphEditorSettings)}");

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            var setting = AssetDatabase.LoadAssetAtPath<FunctionGraphEditorSettings>(path);
            if(setting != null)
                setting.OnRecompile();
        }
    }

    void OnRecompile()
    {
        if (layoutMapping == null)
        {
            Debug.LogWarning($"{name} realaoded mapping as it was set to null after recompile", this);
            CreateMapping();
        }
        else if (serializableMapping != null)
        {
            RemapFromSerializedData();

            TryAddMissing();
        }
    }

    private void TryAddMissing()
    { 
        //mapping = new List<NodeTypeToNodeLayout>();
        var nodeTypes = BaseFuncGraphNode.InstantiableNodeTypes;
        bool newOnesAdded = false;
        foreach (var type in nodeTypes)
        {
            if (!layoutMapping.ContainsKey(type))
            {
                layoutMapping.Add(type, GetLayoutMapping(type));
                newOnesAdded = true;
            }

        }
        if (newOnesAdded)
        {
            Debug.Log("Added previously missing node types");
            CreateSerializableDataAndSave();
        }
    }

    public void DrawMappingInEditor(ref bool show)
    {
        if (layoutMapping != null)
        {
            show = EditorGUILayout.Foldout(show, $"Mapping {layoutMapping.Count}");
            if (show)
            {
                EditorGUILayout.Space();

                EditorGUI.indentLevel++;
                for (int i = 0; i < layoutMapping.Keys.Count; i++)
                {
                    var key = layoutMapping.Keys.ElementAt(i);

                    EditorGUILayout.LabelField(key.ToString(), EditorStyles.boldLabel);
                    var old = layoutMapping[key];
                    layoutMapping[key] = (FunctionGraphEditorNodeLayout)EditorGUILayout.ObjectField(layoutMapping[key] as UnityEngine.Object, typeof(FunctionGraphEditorNodeLayout), false);

                    if (layoutMapping[key] != null)
                    {
                        DrawDummyLayout(layoutMapping[key]);
                    }

                    if (old != layoutMapping[key])
                    {
                        CreateSerializableDataAndSave();
                    }
                }
                EditorGUI.indentLevel--;
            }
        }
    }

    void DrawDummyLayout(FunctionGraphEditorNodeLayout info)
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        Rect r = EditorGUILayout.GetControlRect();

        r.size = new Vector2(25, 25);

        GUI.Box(r, "", info.Style);
        if (info.InConnectionPointsInfo != null)
        {

            for (int i = 0; i < info.InConnectionPointCount; i++)
            {
                info.Draw(r, info.InConnectionPointsInfo[i]);
            }
        }
        if (info.OutConnectionPointsInfo != null)
        {
            for (int i = 0; i < info.OutConnectionPointCount; i++)
            {
                info.Draw(r, info.OutConnectionPointsInfo[i]);
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
    }

    public void CreateSerializableDataAndSave()
    {
        //fill mapping
        serializableMapping = new List<NodeTypeToNodeLayout>();
        foreach (Type key in layoutMapping.Keys)
        {
            var serialMapping = new NodeTypeToNodeLayout();
            serialMapping.SetTypeAndLayout(key, layoutMapping[key]);
            serializableMapping.Add(serialMapping);
        }
        EditorUtility.SetDirty(this);
        
        AssetDatabase.SaveAssets();
    }

    
    public void OnBeforeSerialize()
    {    }

    public void OnAfterDeserialize()
    {
        //restore dictionary
        if (serializableMapping != null)
        {
            if (layoutMapping == null)
            {
                RemapFromSerializedData();
            }
            else
            {
                //only add new ones?
                foreach (var entry in serializableMapping)
                {
                    if (entry.Type == null)
                        continue;
                    if (!layoutMapping.ContainsKey(entry.Type))
                    {
                        layoutMapping.Add(entry.Type, entry.Layout);
                    }
                }
            }
        }
        //throw away "old" data
        //serializableMapping = null;
    }

    void RemapFromSerializedData()
    {
        layoutMapping = new Dictionary<Type, FunctionGraphEditorNodeLayout>();
        //just fill everything
        int i = 0;
        foreach (var entry in serializableMapping)
        {
            if (entry.Type == null)
            {
               //s Debug.Log($"{i} is null");
                continue;
            }
            layoutMapping.Add(entry.Type, entry.Layout);
            i++;
        }
    }

    public FunctionGraphEditorNodeLayout GetLayout(BaseFuncGraphNode node)
    {
        if (layoutMapping == null)
            CreateMapping();

        if (layoutMapping.ContainsKey(node.GetType()))
            return layoutMapping[node.GetType()];
        else if (node is VariableNode || node is ConstantNode || node is FixedConstantNode)
            return defaultNoIn;
        else if (node is SingularChildNode)
            return defaultSingleIn;
        else if (node is DualChildNode)
            return defaultDualIn;
        else if (node is VariableMultiChildNode)
            return defaultMultiIn;
        else if (node is FixedSizeMultiChildNode && node.PossibleChildrenCount == 3)
            return defaultTripleIn;
        return null;
    }

    private void CreateMapping()
    {
        layoutMapping = new Dictionary<Type, FunctionGraphEditorNodeLayout>();
        //mapping = new List<NodeTypeToNodeLayout>();
        var nodeTypes = BaseFuncGraphNode.InstantiableNodeTypes;
        foreach (var type in nodeTypes)
        {
            layoutMapping.Add(type, GetLayoutMapping(type));
            //var map = new NodeTypeToNodeLayout();
            //map.SetTypeAndLayout(type, layoutMapping[type]);
            //mapping.Add(map);
        }
        CreateSerializableDataAndSave();
    }

    private FunctionGraphEditorNodeLayout GetLayoutMapping(Type type)
    {
        if (type == typeof(VariableNode) || type == typeof(ConstantNode) || type.IsSubclassOf(typeof(ConstantNode)) || type.IsSubclassOf(typeof(FixedConstantNode)))
            return defaultNoIn;
        else if (type.IsSubclassOf(typeof(SingularChildNode)))
            return defaultSingleIn;
        else if (type.IsSubclassOf(typeof(DualChildNode)))
            return defaultDualIn;
        else if (type.IsSubclassOf(typeof(VariableMultiChildNode)))
            return defaultMultiIn;
        return null;
    }

    public void Reset()
    {
        CreateMapping();
    }

    [Serializable]
    public struct NodeTypeToNodeLayout
    {
        [SerializeField] string type;
        Type _Type;
        public Type Type
        {
            get
            {
                if (_Type == null)
                {
                    _Type = Type.GetType($"{type}, {Assembly.GetAssembly(typeof(BaseFuncGraphNode)).FullName}");
                }
                return _Type;
            }
        }
        public FunctionGraphEditorNodeLayout Layout;

        public void SetTypeAndLayout(Type t, FunctionGraphEditorNodeLayout layout)
        {
            _Type = t;
            type = t.ToString();
            Layout = layout;
        }


        public override string ToString()
        {
            return $"{type}, some name";
        }
    }

}
