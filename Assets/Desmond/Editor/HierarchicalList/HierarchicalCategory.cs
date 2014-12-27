using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {
public class HierarchicalCategory<T> : HierarchicalListElement<T> where T : IPathable {
    public const int pixelsPerIndent = 20;

    private bool _canMinimize = true;
    private bool _expanded = false;
    private List<HierarchicalCategory<T>> _subCategories = new List<HierarchicalCategory<T>>();
    private List<HierarchicalListElement<T>> _subElements = new List<HierarchicalListElement<T>>();

    public HierarchicalCategory(string name, HierarchicalList<T> parentList, bool canMinimize = true)
        : base(name, parentList) {
        EditorStyles.foldout.richText = true;
        EditorStyles.foldout.normal.textColor = Color.black;
        EditorStyles.label.normal.textColor = Color.black;
        EditorStyles.label.onActive.textColor = Color.black;
        EditorStyles.label.onNormal.textColor = Color.black;
        EditorStyles.label.richText = true;
        _canMinimize = canMinimize;
        if (!_canMinimize) {
            _expanded = true;
        }
    }

    public Rect drawElements(Rect r) {
        foreach (HierarchicalCategory<T> category in _subCategories) {
            if (category.doesMatchFilter) {
                category._expanded = EditorGUI.Foldout(r, category._expanded, category.richName);
                r.y += HierarchicalList<T>.pixelsPerLine;
                if (category._expanded) {
                    r.x += pixelsPerIndent;
                    r.width -= pixelsPerIndent;
                    r = category.drawElements(r);
                    r.x -= pixelsPerIndent;
                    r.width += pixelsPerIndent;
                }
            }
        }
        foreach (HierarchicalListElement<T> element in _subElements) {
            if (element.doesMatchFilter) {
                if (Event.current.type == EventType.MouseDown && r.Contains(Event.current.mousePosition)) {
                    if (_parentList.highlighted == element) {
                        _parentList.selected = element;
                    }
                    _parentList.highlighted = element;
                }
                if (element == _parentList.highlighted) {
                    GUI.Box(r, "");
                }

                GUI.Label(r, element.richName, EditorStyles.label);
                r.y += HierarchicalList<T>.pixelsPerLine;
            }
        }
        return r;
    }

    public override bool setFilter(string filter, bool overrideTrue, bool canHighlight = true) {
        base.setFilter(filter, false, false);

        //If this category matches, all sub category/elements must also be matched
        //unless the filter is ""
        if (filter != "" && doesMatchFilter) {
            overrideTrue = true;
        }

        foreach (HierarchicalCategory<T> category in _subCategories) {
            _matchesFilter |= category.setFilter(filter, overrideTrue, false);
        }
        foreach (HierarchicalListElement<T> element in _subElements) {
            _matchesFilter |= element.setFilter(filter, overrideTrue, true);
        }

        _expanded = (filter != "") || !_canMinimize;

        return _matchesFilter;
    }

    public int getHeight() {
        int height = 0;
        foreach (HierarchicalCategory<T> category in _subCategories) {
            if (category.doesMatchFilter) {
                height++;
                if (category._expanded) {
                    height += category.getHeight();
                }
            }
        }
        foreach (HierarchicalListElement<T> element in _subElements) {
            if (element.doesMatchFilter) {
                height++;
            }
        }
        return height;
    }

    public HierarchicalListElement<T> findElementWithData(T t) {
        foreach (HierarchicalListElement<T> element in _subElements) {
            if (element.data.Equals(t)) {
                return element;
            }
        }
        foreach (HierarchicalCategory<T> element in _subCategories) {
            HierarchicalListElement<T> found = element.findElementWithData(t);
            if (found != null) {
                return found;
            }
        }
        return null;
    }

    public int addElement(string name, T element) {
        int slashIndex = name.IndexOf('/');
        if (slashIndex != -1) {
            string category = name.Substring(0, slashIndex);
            string subName = name.Substring(slashIndex + 1);

            for (int i = 0; i < _subCategories.Count; i++) {
                if (_subCategories[i]._name == category) {
                    return (_subCategories[i] as HierarchicalCategory<T>).addElement(subName, element);
                }
            }

            HierarchicalCategory<T> newCategory = new HierarchicalCategory<T>(category, _parentList, _canMinimize);
            insertIntoList(newCategory, _subCategories);
            return 1 + newCategory.addElement(subName, element);
        } else {
            insertIntoList(new HierarchicalListElement<T>(element, name, _parentList), _subElements);
            return 1;
        }
    }

    private void insertIntoList<K>(K e, List<K> list) where K : HierarchicalListElement<T> {
        for (int i = 0; i < list.Count; i++) {
            if (list[i].name.CompareTo(e.name) > 0) {
                list.Insert(i, e);
                return;
            }
        }
        list.Add(e);
    }
}

}