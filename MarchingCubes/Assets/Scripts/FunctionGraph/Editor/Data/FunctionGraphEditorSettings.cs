using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Function Graph Editor/ Editor Settings")]
public class FunctionGraphEditorSettings : ScriptableObject
{
    [SerializeField] FunctionGraphEditorNodeLayout defaultNoIn = null;
    [SerializeField] FunctionGraphEditorNodeLayout defaultSingleIn = null;
    [SerializeField] FunctionGraphEditorNodeLayout defaultDualIn = null;
    [SerializeField] FunctionGraphEditorNodeLayout defaultMultiIn = null;
    [SerializeField] FunctionGraphEditorNodeLayout defaultTripleIn = null;

    [SerializeField] List<NodeTypeToNodeLayout> nodeToLayout = null;

    Dictionary<Type, FunctionGraphEditorNodeLayout> layoutMapping = null;

    [Serializable]
    public struct NodeTypeToNodeLayout
    {
        [SerializeField] string type;
        Type _Type;
        public Type Type {
            get
            {
                if (_Type == null)
                    _Type = Type.GetType(type);
                return _Type;
            }
        }
        public FunctionGraphEditorNodeLayout Layout;
    }

    public FunctionGraphEditorNodeLayout GetLayout(BaseFuncGraphNode node)
    {
        if (layoutMapping == null)
            CreateMapping();

        if (layoutMapping.ContainsKey(node.GetType()))
            return layoutMapping[node.GetType()];
        else if (node is VariableNode || node is ConstantNode)
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
        if (nodeToLayout != null)
        {
            for (int i = 0; i < nodeToLayout.Count; i++)
            {
                layoutMapping.Add(nodeToLayout[i].Type, nodeToLayout[i].Layout);
            }
        }
    }

    public void RemakeLayoutMapping()
    {
        CreateMapping();
    }
}
