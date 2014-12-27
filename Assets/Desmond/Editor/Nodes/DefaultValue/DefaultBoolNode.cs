using UnityEngine;
using System.Collections;

namespace Desmond { 

public class DefaultBoolNode : DefaultPrimitiveNode {
    public bool value = true;
    public override string primitiveToString() {
        return value ? "true" : "false";
    }
}

}