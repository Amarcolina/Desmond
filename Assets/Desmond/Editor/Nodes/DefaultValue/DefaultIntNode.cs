using UnityEngine;
using System.Collections;

namespace Desmond { 

public class DefaultIntNode : DefaultPrimitiveNode {
    public int value = 0;
    public override string primitiveToString() {
        return value.ToString();
    }
}

}