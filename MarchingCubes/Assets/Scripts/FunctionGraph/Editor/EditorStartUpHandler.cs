using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class EditorStartUpHandler
{
    static EditorStartUpHandler()
    {
        //TODO 
        //create surface gen instance and make useable in editor
        // as well as threaded data requester
        Debug.Log("Start up");
    }
}
