using UnityEngine;
using System.Collections;

namespace Desmond {

public struct ScriptElementKey {
    public Node parentNode;
    public string id;
}

public class ScriptElementStruct {
    public ScriptElementKey structKey;

    public override int GetHashCode() {
        return structKey.GetHashCode();
    }

    public override bool Equals(object obj) {
        return structKey.Equals(obj);
    }
}

}