using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public struct ScriptElementKey {
    public Node parentNode;
    public string id;

    public ScriptElementKey(Node parentNode, string id) {
        this.parentNode = parentNode;
        this.id = id;
    }

    public override string ToString() {
        return parentNode.ToString() + ":" + id;
    }
}

public abstract class ScriptElementStruct {
    public ScriptElementKey structKey;

    public ScriptElementStruct(ScriptElementKey key) {
        structKey = key;
    }

    public override int GetHashCode() {
        return structKey.GetHashCode();
    }

    public override bool Equals(object obj) {
        return structKey.Equals(obj);
    }

    public abstract List<string> generateScriptLines();

    public override string ToString() {
        return structKey.ToString();
    }
}

}