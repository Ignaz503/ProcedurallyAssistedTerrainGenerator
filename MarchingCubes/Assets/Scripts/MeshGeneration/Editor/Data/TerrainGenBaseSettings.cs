using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Terrain Generation Settings")]
public class TerrainGenBaseSettings : ScriptableObject
{
    public int MinChunkResolution;
    public int MaxChunkResolution;
    public string Workspace = "";
}
