using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

[System.Serializable]
public struct ElementPair{
    public Element elementA;
    public Element elementB;

    public ElementPair(Element a, Element b) {
        elementA = a;
        elementB = b;
    }

    public override bool Equals(object obj) {
        ElementPair p = (ElementPair)obj;
        return (elementA == p.elementA && elementB == p.elementB) ||
               (elementA == p.elementB && elementB == p.elementA);
    }

    public override int GetHashCode() {
        return elementA.GetHashCode() + elementB.GetHashCode();
    }
}

public class Element : ScriptableObject {
    public string id;
    public string type;
    public Rect rect;
    public bool visible = true;

    private Color normalLinkColor = new Color(.8f, .8f, .8f);

    public virtual void init(string id, string type) {
        this.id = id;
        this.type = type;
        hideFlags = HideFlags.HideInHierarchy;
        BoardHandler.addAssetToCurrentBoard(this);
    }

    public virtual bool isOnLeft() {
        return true;
    }

    public virtual int getMaxConnections() {
        return -1;
    }

    public virtual int getHeight() {
        return 1;
    }

    public virtual float getWidth() {
        return EditorStyles.label.CalcSize(new GUIContent(id)).x;
    }

    public virtual Texture2D getButtonTexture() {
        return null;
    }

    public virtual void drawElement() { }

    public virtual void drawLink(Element endElement, CurveEnd endA, CurveEnd endB) {
        drawCurve(endA, endB, normalLinkColor);
    }

    protected virtual void drawCurve(CurveEnd a, CurveEnd b, Color color) {
        a.pos.y += 1f;
        b.pos.y += 1f;
        float xMag = Mathf.Abs(a.pos.x - b.pos.x);
        float yMag = Mathf.Abs(a.pos.y - b.pos.y);
        float mag = Mathf.Max(Mathf.Min(70, yMag), 0.32f * (xMag + yMag));
        Handles.DrawBezier(a.pos, b.pos, a.pos + a.dir * mag, b.pos + b.dir * mag, color, null, 4);
    }
}

}