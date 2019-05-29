using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Function Graph Editor/ Node Layout")]
public class FunctionGraphEditorNodeLayout : ScriptableObject
{
    public enum ListType
    {
        In,
        Out
    }

    public GUIStyle Style;
    public float Width;
    public float Height;

    public Vector2 Size
    {
        get{ return new Vector2(Width, Height); }
    }

    public List<InConnectionPointInfo> InConnectionPointsInfo;
    public int InConnectionPointCount { get { return InConnectionPointsInfo.Count; } }
    public List<OutConnectionPointInfo> OutConnectionPointsInfo;
    public int OutConnectionPointCount { get { return OutConnectionPointsInfo.Count; } }

    public ConnectionPointInfo this[ListType list, int idx]
    {
        get
        {
            if (list == ListType.In)
                return InConnectionPointsInfo[idx];
            else
                return OutConnectionPointsInfo[idx];
        }
    }

    public void Draw(Rect r, ConnectionPointInfo info, int i = -1)
    {
        //figure out width and height
        Vector2 size = new Vector2(r.width * info.Width, r.height * info.Height);

        //figure out offset
        Vector2 offset = new Vector2(r.width, r.height);
        offset.Scale(new Vector2(info.UVx, info.UVy));
        offset -= size * .5f;
        Rect rP = new Rect(offset, size);
        rP.position += r.position;

        string s = i != -1 ? $"{i}":"";
        GUI.Box(rP, s, info.Style);
    }

    [Serializable]
    public abstract class ConnectionPointInfo
    {
        public virtual ConnectionPoint.ConnectionPointType Type { get; }

        [Range(0f, 1f)] public float UVx;
        [Range(0f, 1f)] public float UVy;

        [Range(0f, 1f)] public float Width;
        [Range(0f, 1f)] public float Height;

        public GUIStyle Style;
    }

    [Serializable]
    public class InConnectionPointInfo : ConnectionPointInfo
    {
        public enum ConnectionType
        {
            Single,
            Multiple
        }
        [SerializeField] ConnectionType type = ConnectionType.Single;

        public override ConnectionPoint.ConnectionPointType Type { get { return (ConnectionPoint.ConnectionPointType)((int)type + 1); } }
    }

    [Serializable]
    public class OutConnectionPointInfo : ConnectionPointInfo
    {
        public override ConnectionPoint.ConnectionPointType Type { get { return ConnectionPoint.ConnectionPointType.Out; } }
    }

}
