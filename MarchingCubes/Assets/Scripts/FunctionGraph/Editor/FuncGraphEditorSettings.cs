using UnityEngine;

[CreateAssetMenu(menuName = "FuncGraphEditor/Editor Settings Setting")]
public class FuncGraphEditorSettings : ScriptableObject 
{
    public struct NodeToStyle
    {
        public string NodeName;
        public FuncGraphEditorNodeSettings NodeSettings;
    }
}