using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class HierarchicalList<T> where T : IPathable {
    public const int pixelsPerLine = 20;
    public HierarchicalListElement<T> highlighted = null;
    public HierarchicalListElement<T> selected = null;
    public Vector2 size;

    private HierarchicalCategory<T> _allListElements;
    private bool _shouldFocusSearchBox = true;

    private string _searchString = "";

    private Vector2 _scrollPosition = Vector2.zero;
    private Rect _scrollViewRect = new Rect();

    public HierarchicalList(List<T> elements, bool canMinimize = true) {
        updateList(elements, canMinimize);
    }

    public void updateList(List<T> elements, bool canMinimize = true) {
        _allListElements = new HierarchicalCategory<T>("", this, canMinimize);

        foreach (T element in elements) {
            _allListElements.addElement(element.getPath(), element);
        }
    }

    public void recalculateScrollRect() {
        int height = _allListElements.getHeight();
        _scrollViewRect = new Rect(0, 0, 180, height * pixelsPerLine);
    }

    public void select(T t) {
        HierarchicalListElement<T> elementFound = _allListElements.findElementWithData(t);
        selected = elementFound;
        highlighted = elementFound;
    }

    public void open(){
        _searchString = "";
        _allListElements.setFilter(_searchString, false, false);

        highlighted = null;
        selected = null;
        _shouldFocusSearchBox = true;
    }

    public void drawList(Rect rect) {
        GUI.SetNextControlName("SearchField");
        string newSearchString = GUI.TextField(new Rect(rect.x, rect.y, rect.width-1, pixelsPerLine), _searchString);
        if (_shouldFocusSearchBox) {
            EditorGUI.FocusTextInControl("SearchField");
            _shouldFocusSearchBox = false;
        }

        //if (Event.current.isKey) {
         //   EditorGUI.FocusTextInControl("SearchField");
        //    if (Event.current.keyCode == KeyCode.Return) {
        //        selected = highlighted;
        //    }
       // }

        if (newSearchString != _searchString) {
            highlighted = null;
            _searchString = newSearchString == null ? "" : newSearchString.ToLower();
            _allListElements.setFilter(_searchString, false, false);

        }

        recalculateScrollRect();//
        _scrollPosition = GUI.BeginScrollView(new Rect(rect.x, rect.y + pixelsPerLine, rect.width-1, rect.height - pixelsPerLine - 1), _scrollPosition, _scrollViewRect);
        _allListElements.drawElements(new Rect(0, 0, rect.width, pixelsPerLine));
        GUI.EndScrollView(true);
    }

    public void drawListWindow(int id) {
        GUI.DragWindow(new Rect(0, 0, 10000, EditorGUIUtility.singleLineHeight));
        drawList(new Rect(0, EditorGUIUtility.singleLineHeight, size.x, size.y - EditorGUIUtility.singleLineHeight));
    }
}

}