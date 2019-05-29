using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FunctionGraphEditorSettings),true)]
public class FunctionGraphEditorSettingsEditor : Editor
{
    bool show = false;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var tar = target as FunctionGraphEditorSettings;

        tar.DrawMappingInEditor(ref show);

        if (GUILayout.Button("Reset"))
        {
            tar.Reset();
        }

    }
}
