using UnityEngine;

[CreateAssetMenu(menuName = "FuncGraphEditor/Node Setting")]
public class FuncGraphEditorNodeSettings : ScriptableObject
{
    public GUIStyle Style;
    public float Width;
    public float Height;
    public FuncGraphEditorConnectionPointSettings OutSetting;
    public FuncGraphEditorConnectionPointSettings InSetting;
}
