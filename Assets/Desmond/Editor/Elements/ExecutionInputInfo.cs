using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections;

namespace Desmond { 

public class ExecutionInputInfo : Element {

    private static Texture2D _buttonTexture;
    private static GUIStyle _style;

    public override bool isOnLeft() {
        return true;
    }

    public override int getMaxConnections() {
        return -1;
    }

    public override Texture2D getButtonTexture() {
        if (_buttonTexture == null) {
            _buttonTexture = Resources.LoadAssetAtPath<Texture2D>("Assets/Desmond/Textures/InputArrow.png");
        }
        return _buttonTexture;
    }

    public override void drawElement() {
        if (_style == null) {
            _style = new GUIStyle();
            _style.alignment = TextAnchor.MiddleLeft;
            _style.normal.textColor = new Color(0.8f, 1.0f, 0.8f);
        }
        GUI.Label(rect, id, _style);
    }

}

}