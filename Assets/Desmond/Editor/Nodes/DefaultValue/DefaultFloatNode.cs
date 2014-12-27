using UnityEngine;
using System.Collections;

namespace Desmond { 

public class DefaultFloatNode : DefaultPrimitiveNode {
    public float value = 0.0f;
    public override string primitiveToString() {
        return value.ToString();
    }
}

}