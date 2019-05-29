using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Function Graph Editor/ Editor Settings")]
public class FunctionGraphEditorSettings : ScriptableObject
{
#if UNITY_EDITOR
    static event Action Recompile;
#endif

    [SerializeField] FunctionGraphEditorNodeLayout defaultNoIn = null;

    [SerializeField] FunctionGraphEditorNodeLayout defaultSingleIn = null;
    [SerializeField] FunctionGraphEditorNodeLayout defaultDualIn = null;
    [SerializeField] FunctionGraphEditorNodeLayout defaultMultiIn = null;
    [SerializeField] FunctionGraphEditorNodeLayout defaultTripleIn = null;

    Dictionary<Type, FunctionGraphEditorNodeLayout> layoutMapping = null;
    //[SerializeField]List<NodeTypeToNodeLayout> mapping;

#if UNITY_EDITOR
    private void Awake()
    {
        Recompile += OnRecompile;
        CreateMapping();     
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        Recompile?.Invoke();
    }

    void OnRecompile()
    {
        if (layoutMapping == null)
        {
            Debug.LogWarning($"{name} realaoded mapping as it was set to null after recompile", this);
            CreateMapping();
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
                    if (old != layoutMapping[key])
                    {
                        //Debug.Log($"Saving Asset {name}",this);
                        AssetDatabase.SaveAssets();
                    }
                }
                EditorGUI.indentLevel--;
            }
        }
    }


#endif

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
        var nodeTypes = Assembly.GetAssembly(typeof(BaseFuncGraphNode)).GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(BaseFuncGraphNode)));
        foreach (var type in nodeTypes)
        {
            layoutMapping.Add(type, GetLayoutMapping(type));
            //var map = new NodeTypeToNodeLayout();
            //map.SetTypeAndLayout(type, layoutMapping[type]);
            //mapping.Add(map);
        }

        AssetDatabase.SaveAssets();
    }

    private FunctionGraphEditorNodeLayout GetLayoutMapping(Type type)
    {
        if (type == typeof(VariableNode) || type == typeof(ConstantNode) || type.IsSubclassOf(typeof(FixedConstantNode)))
            return defaultNoIn;
        else if (type.IsSubclassOf(typeof(SingularChildNode)))
            return defaultSingleIn;
        else if (type.IsSubclassOf(typeof(DualChildNode)))
            return defaultDualIn;
        else if (type.IsSubclassOf(typeof(VariableMultiChildNode)))
            return defaultMultiIn;
        return null;
    }

    public void Remap()
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
                    _Type = Type.GetType(type);
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

    }

}
