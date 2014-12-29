using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections;

namespace Desmond { 

public class DataOutInfo : Element{

    private static Texture2D _buttonTexture;
    private static GUIStyle _style;

    public override bool isOnLeft() {
        return false;
    }

    public override int getMaxConnections() {
        return -1;
    }

    public override Texture2D getButtonTexture() {
        if (_buttonTexture == null) {
            _buttonTexture = Resources.LoadAssetAtPath<Texture2D>("Assets/Desmond/Textures/OutputDot.png");
        }
        return _buttonTexture;
    }

    public override bool drawElement() {
        if (_style == null) {
            _style = new GUIStyle();
            _style.alignment = TextAnchor.MiddleRight;
            _style.normal.textColor = new Color(0.8f, 1.0f, 0.8f);
        }
        GUI.Label(rect, id, _style);
        return false;
    }
}

}