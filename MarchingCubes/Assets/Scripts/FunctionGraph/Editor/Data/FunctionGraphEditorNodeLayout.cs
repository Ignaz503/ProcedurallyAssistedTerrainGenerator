using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Function Graph Editor/ Node Layout")]
public class FunctionGraphEditorNodeLayout : ScriptableObject
{
    public GUIStyle Style;
    public float Width;
    public float Height;
    public List<ConnectionPointInfo> connectionPointsInfo;
    public int ConnectionPointCount { get { return connectionPointsInfo.Count; } }
    
    [Serializable]
    public struct ConnectionPointInfo
    {
        public ConnectionPoint.ConnectionPointType Type;

        [Range(0f, 1f)] public float UVx;
        [Range(0f, 1f)] public float UVy;

        [Range(0f, 1f)] public float Width;
        [Range(0f, 1f)] public float Height;

        public GUIStyle Style;
    }

    public ConnectionPointInfo this[int i]
    {
        get
        {
            return connectionPointsInfo[i];
        }
    }
}
