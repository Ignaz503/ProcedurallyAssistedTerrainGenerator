//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEditor;
//using UnityEditor.SceneManagement;

//[CustomEditor(typeof(ThreadedDataRequester))]
//public class ThreadedDataReuestEditor : Editor
//{
//    [MenuItem("GameObject/Create Threaded DataReuester For Editor")]
//    public static void  CreateThreadedDataReuester()
//    {
//        ThreadedDataRequester.Instance.EditorHello();
//    }

//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();

//        ThreadedDataRequester tar = (ThreadedDataRequester)target;

//        if (GUILayout.Button("Make Useable In Editor"))
//        {
//            tar.MakeUseableInEditor();
//        }
//    }
//}