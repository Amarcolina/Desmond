using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Desmond { 

public class DesmondWindow : EditorWindow {
    public const string EDITOR_TEXTURE_FOLDER = "Assets/Desmond/Editor/Textures/";

    private static DesmondWindow _currentWindow;

    private DesmondBoard board;

    private Vector2 offset;

    private Node selectedNode = null;
    private Element selectedElement = null;

    private bool _hasNodeListOpen = false;
    private Rect _nodeListRect = new Rect(0, 0, 200, 400);
    private HierarchicalList<NodeFactory> _nodeList;

    public DesmondSidebar _sidebar = new DesmondSidebar();
    public DesmondToolbar _toolbar = new DesmondToolbar();

    private static GUIStyle _windowStyle;
    private static Texture2D gridTexture;

    public static GUIStyle windowStyle {
        get {
            if (_windowStyle == null) {
                createWindowStyle();
            }
            return _windowStyle;
        }
    }

    public HierarchicalList<NodeFactory> nodeList {
        get {
            if (_nodeList == null) {
                _nodeList = new HierarchicalList<NodeFactory>(NodeFactory.getAllFactories());
            }
            return _nodeList;
        }
    }

    private static void createWindowStyle() {
        gridTexture = Resources.LoadAssetAtPath<Texture2D>(EDITOR_TEXTURE_FOLDER + "Grid.bmp");
        Texture2D normalTex = Resources.LoadAssetAtPath<Texture2D>(EDITOR_TEXTURE_FOLDER + "WindowNormal.png");
        Texture2D highlightTex = Resources.LoadAssetAtPath<Texture2D>(EDITOR_TEXTURE_FOLDER + "WindowHighlight.png");
        _windowStyle = new GUIStyle();
        _windowStyle.normal.background = normalTex;
        _windowStyle.normal.textColor = Color.black;
        _windowStyle.onNormal.background = highlightTex;
        _windowStyle.onNormal.textColor = Color.black;
        _windowStyle.alignment = TextAnchor.UpperCenter;
        _windowStyle.border = new RectOffset(19, 19, 17, 5);
        _windowStyle.padding = new RectOffset(0, 0, 1, 0);
    }


    [MenuItem("Desmond/Editor")]
    static void openDesmondWindow() {
        createWindowStyle();
        getCurrent();
    }

    public static DesmondWindow getCurrent() {
        if (_currentWindow == null) {
            _currentWindow = (DesmondWindow)EditorWindow.GetWindow(typeof(DesmondWindow));
            _currentWindow.autoRepaintOnSceneChange = true;
            _currentWindow.minSize = new Vector2(800, 440);
        }
        return _currentWindow;
    }

    public void OnGUI() {
        if (gridTexture == null) {
            createWindowStyle();
        }

        if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseUp) {
            Repaint();
        }

        try {
            GUI.BeginGroup(new Rect(0, 0, DesmondSidebar.width, position.height));
            _sidebar.doSidebar(new Rect(0, 0, DesmondSidebar.width, position.height));
            GUI.EndGroup();

            GUI.BeginGroup(new Rect(DesmondSidebar.width, 0, position.width - DesmondSidebar.width, DesmondToolbar.height));
            _toolbar.doTopBar(new Rect(0, 0, position.width, DesmondToolbar.height));
            GUI.EndGroup();

            GUI.BeginGroup(new Rect(DesmondSidebar.width, DesmondToolbar.height, 10000, 10000));

            GUI.color = Color.gray;
            GUI.DrawTextureWithTexCoords(new Rect(-8, 0, 1280, 1280), gridTexture, new Rect(0, 0, 20, 20));
            GUI.color = Color.white;

            board = BoardList.getSelectedBoard();
            if (board != null) {
                doBoardGUI();
            }

            GUI.EndGroup();
        } catch (System.Exception e){
            Close();
            throw e;
        }
    }

    public void doBoardGUI() {
        if (Event.current.type == EventType.mouseDrag && Event.current.button == 2) {
            offset += Event.current.delta;

            Vector2 appliedOffset = Vector2.zero;
            if (Mathf.Abs(offset.x) >= 16.0f) {
                appliedOffset.x = Mathf.Sign(offset.x) * 16.0f;
            }
            if (Mathf.Abs(offset.y) >= 16.0f) {
                appliedOffset.y = Mathf.Sign(offset.y) * 16.0f;
            }

            offset -= appliedOffset;

            foreach(Node node in board.nodesInBoard){
                node.rect.position += appliedOffset;
            }
            Repaint();
        }

        drawLinks();

        BeginWindows();
        drawNodes();
        drawComponentList();
        EndWindows();

        if (selectedElement!=null && Event.current.type == EventType.MouseUp && Event.current.button == 0) {
            selectedElement = null;
            Repaint();
        }

        drawSelectingLink();
    }

    public void drawLinks() {
        foreach (Node node in board.nodesInBoard) {
            if (node.isVisible) {
                node.drawLinks();
            }
        }
    }

    public void drawNodes() {
        int id = 1;

        foreach (Node node in board.nodesInBoard) {
            if (node.isVisible) {
                node.rect = GUI.Window(id, node.rect, node.doWindow, node.name, windowStyle);
            }

            Vector2 p = node.rect.position;
            p.x = (int)(p.x / 16.0f) * 16.0f;
            p.y = (int)(p.y / 16.0f) * 16.0f;
            node.rect.position = p;
            id++;
        }
    }

    public void drawComponentList() {
        if (Event.current.type == EventType.MouseDown && Event.current.button == 1) {
            _hasNodeListOpen = true;
            _nodeListRect = new Rect(Event.current.mousePosition.x - 8, Event.current.mousePosition.y - 20, 250, 350);
            nodeList.open();
        }

        if (_hasNodeListOpen) {
            nodeList.size = new Vector2(250, 350);
            _nodeListRect = GUI.Window(0, _nodeListRect, nodeList.drawListWindow, "");
            HierarchicalListElement<NodeFactory> element = nodeList.selected;

            if (element != null || !_nodeListRect.Contains(Event.current.mousePosition)) {
                if (element != null) {
                    createNewNode(element.data);
                }
                _hasNodeListOpen = false;
                Repaint();
            }
        }
    }
    
    public void createNewNode(NodeFactory factory) {
        Node newNode = factory.createNode();

        newNode.rect.position = new Vector2(Event.current.mousePosition.x - Node.SIDE / 2.0f, Event.current.mousePosition.y - 8);

        board.nodesInBoard.Add(newNode);
    }

    public void drawSelectingLink() {
        if (selectedElement != null) {
            if (Event.current.isMouse) {
                Repaint();
            }

            CurveEnd mouseEnd = new CurveEnd(Event.current.mousePosition, selectedElement.isOnLeft() ? Vector2.right : -Vector2.right);
            selectedNode.drawLink(selectedElement, mouseEnd);
        }
    }

    public void selectElement(Element element, Node node) {
        if(selectedElement != null){
            if (element == selectedElement) {
                if (element is ConnectableElement) {
                    (element as ConnectableElement).disconnectFromAll();
                }
            }else {
                tryConnectElements(selectedElement, selectedNode, element, node);
            }
            selectedElement = null;
            selectedNode = null;
            Repaint();
        }else{
            selectedElement = element;
            selectedNode = node;
        }
    }

    public bool tryConnectElements(Element elementA, Node nodeA, Element elementB, Node nodeB) {

        if (elementA is ConnectableElement) {
            if ((elementA as ConnectableElement).tryConnect(new ElementConnection(elementB, nodeA, nodeB))) {
                return true;
            }
        }
        if (elementB is ConnectableElement) {
            return (elementB as ConnectableElement).tryConnect(new ElementConnection(elementA, nodeB, nodeA));
        }
        return false;
    }
}

public struct CurveEnd {
    public Vector2 pos;
    public Vector2 dir;

    public CurveEnd(Vector2 pos, Vector2 dir) {
        this.pos = pos;
        this.dir = dir;
    }
}

}