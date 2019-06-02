using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Function Graph Editor/Connection Point Style")]
public class ConnectionPointStyle : ScriptableObject
{
    public Texture2D Normal;
    public Color TextColor;

    public Texture2D Hover;
    public Color HoverTextColor;

    public Font Font;
    public TextAnchor Alignment;

    public GUIStyle Style
    {
        get
        {
            var style = new GUIStyle();

            style.normal = new GUIStyleState()
            {
                background = Normal,
                textColor = TextColor
            };

            style.active = style.normal;

            style.hover = new GUIStyleState()
            {
                background = Hover,
                textColor = HoverTextColor
            };

            style.onHover = style.hover;

            style.font = Font;
            style.alignment = Alignment;


            return style;
        }
    }

}
