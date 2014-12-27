using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrderedDictionary<T, K> : IEnumerable<K> {
    private List<K> list = new List<K>();
    private Dictionary<T, int> map = new Dictionary<T, int>();

    public bool tryAdd(T t, K k) {
        if (!map.ContainsKey(t)) {
            map[t] = list.Count;
            list.Add(k);
            return true;
        }
        return false;
    }

    public bool containsKey(T t) {
        return map.ContainsKey(t);
    }

    public int count() {
        return list.Count;
    }

    public void clear() {
        list.Clear();
        map.Clear();
    }

    public IEnumerator<K> GetEnumerator() {
        return list.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
        return this.GetEnumerator();
    }

    public K this[T t] {
        get {
            return list[map[t]];
        }
    }

}
