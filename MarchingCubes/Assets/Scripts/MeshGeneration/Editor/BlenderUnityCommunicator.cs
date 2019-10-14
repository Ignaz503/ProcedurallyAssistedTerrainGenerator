using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEditor.Formats.Fbx.Exporter;

public class BlenderUnityCommunicator
{
    public static string blenderPath = @"C:\Program Files\Blender Foundation\Blender";


    string hostIP;
    int PORT;

    public BlenderUnityCommunicator(string hostIP, int pORT, string pathBlender)
    {
        this.hostIP = hostIP ?? throw new ArgumentNullException(nameof(hostIP));
        PORT = pORT;
        blenderPath = pathBlender;
    }

    public void ExportChunksForEdit(GameObject[] toEdit, string filePath)
    {
        for (int i = 0; i < toEdit.Length; i++)
        {
            ModelExporter.ExportObject(filePath + $"/chunk{i}.fbx", toEdit[i]);
        }
    }

    public void StartBlender()
    {
        Process.Start(blenderPath);
    }


}
