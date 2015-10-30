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
            _buttonTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(DesmondWindow.EDITOR_TEXTURE_FOLDER + "OutputDot.png");
        }
        return _buttonTexture;
    }

    public override void drawElement() {
        if (_style == null) {
            _style = new GUIStyle();
            _style.alignment = TextAnchor.MiddleRight;
            _style.normal.textColor = new Color(0.8f, 1.0f, 0.8f);
        }
        GUI.Label(rect, id, _style);
    }
}

}