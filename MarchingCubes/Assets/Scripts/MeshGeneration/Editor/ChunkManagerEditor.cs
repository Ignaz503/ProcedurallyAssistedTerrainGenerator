using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ChunkManager))]
public class ChunkManagerEditor : Editor
{

    public void OnSceneGUI()
    {
        ChunkManager manager = target as ChunkManager;

        Handles.color = Handles.xAxisColor;
        Handles.CubeHandleCap(0, Vector3.zero, Quaternion.identity, 5f, EventType.Repaint);
    }

}

