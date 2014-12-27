using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public class Node : ScriptableObject, ISerializationCallbackReceiver {
    public const int LINE = 16;
    public const int SIDE = 100;

    public Rect rect;

    public bool isStatic;
    public bool isGameObject;
    public bool isVisible = true;
    public bool isMinimized = false;

    public GameObject gameObjectInstance = null;

    public List<Element> elements = new List<Element>();
    private Dictionary<string, Element> _idToElement = null;

    public Dictionary<string, Element> idToElement {
        get {
            if (_idToElement == null) {
                _idToElement = new Dictionary<string, Element>();
                foreach (Element e in elements) {
                    _idToElement[e.id] = e;
                }
            }
            return _idToElement;
        }
    }

    public virtual void generateElements() {
        BoardHandler.addAssetToCurrentBoard(this);  
    }

    public void OnDestroy() {
        foreach (Element element in elements) {
            if (element != null) {
                DestroyImmediate(element, true);
            }
        }
    }

    public virtual Element getElement(string id) {
        return idToElement[id];
    }

    public virtual List<FieldStruct> getFieldStructs() {
        return new List<FieldStruct>();
    }

    public virtual List<MethodStruct> getMethodStructs() {
        return new List<MethodStruct>();
    }

    public virtual List<GenericCodeStruct> getGenericCodeStructs() {
        return new List<GenericCodeStruct>();
    }

    public virtual List<ExpressionMethodStruct> getExpressionStructs() {
        return new List<ExpressionMethodStruct>();
    }

    public static Rect rectSlide(Rect r, float amount) {
        float x = amount > 0 ? r.x + amount : r.x;
        float width = r.width - Mathf.Abs(amount);
        return new Rect(x, r.y, width, r.height);
    }

    public static Rect rectSlide(Rect r, float amount, bool isOnLeft) {
        return rectSlide(r, isOnLeft ? amount : -amount);
    }

    public Rect getButtonRect(Element e, bool global = true) {
        Rect r = new Rect(e.rect.position.x, e.rect.position.y, LINE, LINE);

        if (isMinimized) {
            r.y = LINE;
        }

        if (global) {
            r.position += rect.position;
        }

        if (!e.isOnLeft()) {
            r.x += SIDE;
        } else {
            r.x -= LINE;
        }
        return r;
    }

    public CurveEnd getCurveEnd(Element e) {
        return new CurveEnd(getButtonRect(e).center, Vector2.right * (e.isOnLeft() ? -1 : 1));
    }

    public void updateHeight() {
        bool hasLeft = false;
        bool hasRight = false;

        foreach (Element e in elements) {
            if (!e.visible) {
                continue;
            }

            if (e.isOnLeft()) {
                hasLeft = true;
            }
            if (!e.isOnLeft()) {
                hasRight = true;
            }
        }

        Rect leftRect = new Rect(LINE, LINE, SIDE, LINE);
        Rect rightRect = leftRect;

        if (hasLeft) {
            rightRect.position += new Vector2(SIDE, 0);
            if (isGameObject) {
                leftRect.y += LINE;
            }
        } else {
            rightRect.y += LINE;
        }

        foreach (Element e in elements) {
            if (!e.visible) {
                continue;
            }

            if (e.isOnLeft()) {
                e.rect = leftRect;
                leftRect.y += e.getHeight();
            } else {
                e.rect = rightRect;
                rightRect.y += e.getHeight();
            }
        }

        if (hasLeft && hasRight) {
            rect.width = SIDE * 2 + LINE * 2;
        } else {
            rect.width = SIDE + LINE * 2;
        }

        if (isMinimized) {
            rect.height = LINE * 2 + 4;
        } else {
            rect.height = Mathf.Max(leftRect.y, rightRect.y) + 4;
        }
    }

    public void drawLinks() {
        foreach (ConnectableElement a in elements.FindAll(element => element is ConnectableElement)) {
            CurveEnd endA = getCurveEnd(a);
            foreach(ElementConnection connection in a.connections.FindAll(connection => connection.originNode == this)){
                if (connection.connectedNode.isVisible) {
                    CurveEnd endB = connection.connectedNode.getCurveEnd(connection.connectedElement);
                    a.drawLink(connection.connectedElement, endA, endB);
                }
            }
        }
    }

    public void drawLink(Element elementA, CurveEnd endB) {
        CurveEnd endA = getCurveEnd(elementA);
        elementA.drawLink(null, endA, endB);
    }

    public virtual void doWindow(int windowID) {
        isMinimized = !EditorGUI.Foldout(new Rect(8, 1, LINE, LINE), !isMinimized, GUIContent.none);
        GUI.DragWindow(new Rect(0, 0, 10000, EditorGUIUtility.singleLineHeight));

        updateHeight();

        if (!isMinimized && isGameObject) {
            DesmondBoard currentBoard = BoardList.getSelectedBoard();
            bool allowSceneObject = currentBoard.boardType == DesmondBoardType.SCENE_BOARD;
            gameObjectInstance = (GameObject)EditorGUI.ObjectField(new Rect(LINE, LINE, SIDE, LINE), gameObjectInstance, typeof(GameObject), allowSceneObject);
        }

        foreach (Element e in elements) {
            if (!e.visible) {
                continue;
            }

            if (!isMinimized) {
                e.drawElement();
            }

            Texture2D buttonTexture = e.getButtonTexture();
            if (buttonTexture != null) {
                Rect buttonRect = getButtonRect(e, false);
                GUI.Box(buttonRect, new GUIContent("", e.type), GUIStyle.none);
                GUI.DrawTexture(buttonRect, buttonTexture);
                if (buttonRect.Contains(Event.current.mousePosition) &&
                    (Event.current.type == EventType.MouseUp || Event.current.type == EventType.MouseDown) &&
                    Event.current.button == 0) {
                    if (isMinimized) {
                        isMinimized = false;
                    } else {
                        DesmondWindow.getCurrent().selectElement(e, this);
                    }
                }
            }
        }
    }

    public virtual void OnBeforeSerialize() {

    }

    public virtual void OnAfterDeserialize() {

    }
}

}