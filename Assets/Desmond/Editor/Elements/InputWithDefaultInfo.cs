using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class InputWithDefaultInfo : ConnectableElement{
    private Color dataLinkColor = new Color(.4f, .4f, .9f);
    private Color dataLinkCastColor = new Color(.8f, .4f, .8f);
    private Color dataLinkUnsafeColor = new Color(1.0f, .4f, .4f);

    private static Texture2D _buttonTexture = null;
    private static GUIStyle _style;

    public DefaultValueNode defaultValue;
    public Element valueElement;
    public ElementConnection defaultConnection;

    public override void init(string id, string type, Node parentNode) {
        base.init(id, type, parentNode);
        defaultValue = DefaultValueNode.getDefaultValueNode(type);
        if (defaultValue != null) {
            valueElement = (defaultValue as DefaultValueNode).getElement("out");
            defaultConnection = new ElementConnection(valueElement, parentNode, defaultValue as DefaultValueNode);
            connections.Add(defaultConnection);
        }
    }

    public override float getWidth() {
        return Node.SIDE;
    }

    public override bool isOnLeft() {
        return true;
    }

    public override bool canConnectTo(ElementConnection connection) {
        return (connection.destinationElement is DataOutInfo) && ((type == connection.destinationElement.type) ||
               TypeConversion.canConvertTo(connection.destinationElement.type, type)); 
    }

    public override int getMaxConnections() {
        return 1;
    }

    public override bool tryConnect(ElementConnection connection) {
        if (canConnectTo(connection)) {
            parentNode.requestLayoutUpdate();
            connections.Clear();
            connections.Add(connection);
            return true;
        }
        return false;
    }

    public override void disconnectFrom(ElementConnection other) {
        connections.Remove(other);
        if (connections.Count == 0) {
            connections.Add(defaultConnection);
            parentNode.requestLayoutUpdate();
        }
    }

    public override void disconnectFromAll() {
        connections.Clear();
        connections.Add(defaultConnection);
        parentNode.requestLayoutUpdate();
    }

    public override int getHeight() {
        if (defaultValue != null && connections[0].Equals(defaultConnection)) {
            return 2;
        } else {
            return 1;
        }
    }

    public override Texture2D getButtonTexture() {
        if (_buttonTexture == null) {
            _buttonTexture = Resources.LoadAssetAtPath<Texture2D>(DesmondWindow.EDITOR_TEXTURE_FOLDER + "InputDot.png");
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

        if (defaultValue != null && connections[0].Equals(defaultConnection)) {
            defaultValue.drawDefaultProperty(new Rect(rect.x, rect.y + Node.LINE, rect.width, rect.height));
        }
    }

    public override void drawLink(Element elementB, CurveEnd endA, CurveEnd endB) {
        if (elementB != null && elementB.type == "object") {
            drawCurve(endA, endB, dataLinkUnsafeColor);
        } else if (elementB == null || elementB.type == type) {
            drawCurve(endA, endB, dataLinkColor);
        } else {
            drawCurve(endA, endB, dataLinkCastColor);
        }
    }
}

}
