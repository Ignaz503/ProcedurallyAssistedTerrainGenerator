using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AssetTracker : UnityEditor.AssetModificationProcessor
{
    static void OnWillCreateAsset(string assetName)
    {
        Debug.Log($"Will Create Asset with name {assetName}");  
    }

    static AssetDeleteResult OnWillDeleteAsset(string name, RemoveAssetOptions options)
    {
        Debug.Log($"Will destroy asset with name {name} and options {options}");

        AssetDeleteResult delRes = AssetDeleteResult.DidDelete;

        return delRes;
    }

    private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
    {
        Debug.Log("Source path: " + sourcePath + ". Destination path: " + destinationPath + ".");
        AssetMoveResult assetMoveResult = AssetMoveResult.DidMove;

        // Perform operations on the asset and set the value of 'assetMoveResult' accordingly.
       

        return assetMoveResult;
    }

    static string[] OnWillSaveAssets(string[] paths)
    {
        Debug.Log("OnWillSaveAssets");
        foreach (string path in paths)
            Debug.Log(path);
        return paths;
    }

}

