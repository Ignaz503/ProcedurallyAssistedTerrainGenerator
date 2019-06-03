using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct FunctionGraphEditorNodeSerializable 
{
    public string EditorNodeType;
    public Rect NodeRect;
    public string GraphNodeType;
    public string NodeValue;

    public List<FunctionGraphEditorNodeSerializable> Children;
}
