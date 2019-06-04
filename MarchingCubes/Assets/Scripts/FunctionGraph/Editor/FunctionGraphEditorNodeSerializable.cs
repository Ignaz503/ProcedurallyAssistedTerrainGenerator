using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct FunctionGraphEditorNodeSerializable 
{
    public string EditorNodeType;
    public Vector2 NodePosition;
    public string GraphNodeType;
    public string NodeValue;

    public int ToIdx;
    public int FromIdx;

    public int ParentIndex;
}
