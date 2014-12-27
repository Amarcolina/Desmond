using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond{

public class ExecutionOutInfo : ConnectableElement {

    private static GUIStyle _style;
    private static Texture2D _buttonTexture = null;
    private static GUIStyle _linkButtonStyle = null;
    private Color sequenceLinkColor = new Color(.9f, .8f, .4f);

    public override bool isOnLeft() {
        return false;
    }

    public override bool canConnectTo(ElementConnection other) {
        return (other.connectedElement is ExecutionInputInfo);
    }

    public override int getMaxConnections() {
        return -1;
    }

    public override float getHeight() {
        return Node.LINE;
    }

    public override Texture2D getButtonTexture() {
        if (_buttonTexture == null) {
            _buttonTexture = Resources.LoadAssetAtPath<Texture2D>("Assets/Desmond/Textures/OutputArrow.png");
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

    public override void drawLink(Element elementB, CurveEnd endA, CurveEnd endB) {
        if (_linkButtonStyle == null) {
            _linkButtonStyle = new GUIStyle();
            _linkButtonStyle.normal.background = Resources.LoadAssetAtPath<Texture2D>("Assets/Desmond/Textures/Button.png");
            _linkButtonStyle.normal.textColor = new Color(0.8f, 1.0f, 0.8f);
            _linkButtonStyle.alignment = TextAnchor.MiddleCenter;
            _linkButtonStyle.padding = new RectOffset(0, 2, 0, 2);
        }

        if (connections.Count > 1) {
            drawCurve(endA, endB, sequenceLinkColor);
            int index = -1;
            for (int i = 0; i < connections.Count; i++) {
                if (connections[i].connectedElement == elementB) {
                    index = i;
                    break;
                }
            }

            if (index != -1) {
                Vector2 pos = (endA.pos + endB.pos) / 2.0f;
                if (GUI.Button(new Rect(pos.x - 10, pos.y - 10, 20, 20), index + "", _linkButtonStyle)) {
                    connections.RemoveAt(index);
                }
            }
        } else {
            base.drawLink(elementB, endA, endB);
        }
    }
}

}