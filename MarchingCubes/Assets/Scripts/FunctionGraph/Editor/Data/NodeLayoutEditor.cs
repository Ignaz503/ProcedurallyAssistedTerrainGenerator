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

        if (tar.connectionPointsInfo != null)
        {
            Rect r = EditorGUILayout.GetControlRect();

            r.size = new Vector2(tar.Width, tar.Height);

            GUI.Box(r,"",tar.Style);

            foreach (var info in tar.connectionPointsInfo)
            {
                //figure out width and height
                Vector2 size = new Vector2(r.width * info.Width, r.height * info.Height);

                //figure out offset
                Vector2 offset = new Vector2(r.width, r.height);
                offset.Scale(new Vector2(info.UVx, info.UVy));
                offset -= size * .5f;
                Rect rP = new Rect(offset, size);
                rP.position += r.position;

                GUI.Box(rP,"",info.Style);

            }
        }
    }
}
