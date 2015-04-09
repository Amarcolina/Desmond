using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScriptElementSet<T> : IEnumerable<T> {
    private List<T> list = new List<T>();
    private Dictionary<T, int> map = new Dictionary<T, int>();

    public bool tryAdd(T t) {
        if (!map.ContainsKey(t)) {
            map[t] = list.Count;
            list.Add(t);
            return true;
        }
        return false;
    }

    public void addAll(ICollection<T> collection) {
        foreach (T t in collection) {
            tryAdd(t);
        }
    }

    public bool contains(T t) {
        return map.ContainsKey(t);
    }

    public int count() {
        return list.Count;
    }

    public void clear() {
        list.Clear();
        map.Clear();
    }

    public IEnumerator<T> GetEnumerator() {
        return list.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
        return this.GetEnumerator();
    }
}
