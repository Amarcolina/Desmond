using UnityEngine;
using System.Collections;

namespace Desmond {

public class HierarchicalListElement<T> where T : IPathable {
    protected HierarchicalList<T> _parentList;
    protected T _data;
    protected string _name;
    protected string _richName;
    protected bool _matchesFilter = true;
    protected int _matchValue = 0;

    public HierarchicalListElement(string name, HierarchicalList<T> parentList) {
        _name = name;
        _richName = name;
        _parentList = parentList;
    }

    public HierarchicalListElement(T data, string name, HierarchicalList<T> parentList) {
        _data = data;
        _name = name;
        _richName = name;
        _parentList = parentList;
    }

    public string name {
        get { return _name; }
    }

    public string richName {
        get { return _richName; }
    }

    public bool doesMatchFilter {
        get { return _matchesFilter; }
    }

    public int getMatchValue {
        get { return _matchValue; }
    }

    public T data {
        get { return _data; }
    }

    public virtual bool setFilter(string filter, bool overrideTrue, bool canHighlight = true) {
        int matchIndex = _name.ToLower().IndexOf(filter.ToLower());
        _matchesFilter = matchIndex != -1;

        if (_matchesFilter && filter != "") {
            string nameStart = _name.Substring(0, matchIndex);
            string nameMiddle = _name.Substring(matchIndex, filter.Length);
            string nameEnd = _name.Substring(matchIndex + filter.Length);
            _richName = nameStart + "<color=red>" + nameMiddle + "</color>" + nameEnd;
        } else {
            _richName = _name;
        }

        if (canHighlight && (_matchesFilter || overrideTrue)) {
            _matchValue = _matchesFilter ? StringHelper.findSimilarity(_name.ToLower(), filter) : 999;
            if (_parentList.highlighted == null || _matchValue < _parentList.highlighted._matchValue) {
                _parentList.highlighted = this;
            }
        }
        _matchesFilter |= overrideTrue;
        return _matchesFilter;
    }
}

}