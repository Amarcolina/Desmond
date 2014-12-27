using UnityEngine;
using System.Collections;

namespace Desmond {

public class DefaultVector2Node : DefaultPrimitiveNode {
    public Vector2 value = Vector2.zero;
    public override string primitiveToString() {
        return "(new Vector2(" + value.x + "," + value.y + "))";
    }
}

}