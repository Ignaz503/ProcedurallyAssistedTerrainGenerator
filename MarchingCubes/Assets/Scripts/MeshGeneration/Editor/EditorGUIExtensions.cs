using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class EditorGUIExtensions
{
    public static void Space(int amount = 1)
    {
        for (int i = 0; i < amount; i++)
        {
            EditorGUILayout.Space();
        }
    }
}
