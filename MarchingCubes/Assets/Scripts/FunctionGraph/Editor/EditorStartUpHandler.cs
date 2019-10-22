using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BlenderUnityCommunication;

[InitializeOnLoad]
public class EditorStartUpHandler
{
    static EditorStartUpHandler()
    {
        //TODO 
        //create surface gen instance and make useable in editor
        // as well as threaded data requester
        Debug.Log("Start up");
        ThreadedDataRequester.Instance.EditorHello();



        //var msg = new TCPMessage() { Type = TCPMessage.MsgType.Test, Info = "info", PayLoad = "payload" };
        ////string j = msg.Stringify(true);
        //string j = msg.Stringify();
        //Debug.Log(j);

        //var obj = TCPMessage.Deserialize(j, true);
        //obj.Log(Debug.unityLogger);

    }
}
