using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FunctionGraphEditorNodeLayout),true)]
public class NodeLayoutEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        FunctionGraphEditorNodeLayout tar = target as FunctionGraphEditorNodeLayout;

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        Rect r = EditorGUILayout.GetControlRect();

        r.size = new Vector2(tar.Width, tar.Height);
        //r.size = new Vector2(150f, 150f);

        GUI.Box(r, "", tar.Style);

        if (tar.InConnectionPointsInfo != null)
        {
            for (int i = 0; i < tar.InConnectionPointCount; i++)
            {
                tar.Draw(r, tar.InConnectionPointsInfo[i], i);
            }
        }
        if (tar.OutConnectionPointsInfo != null)
        {
            for (int i = 0; i < tar.OutConnectionPointCount; i++)
            {
                tar.Draw(r, tar.OutConnectionPointsInfo[i], i);
            }
        }

        for (int i = 0; i < tar.Height / 5f; i++)
        {
            EditorGUILayout.Space();
        }

    }


}
