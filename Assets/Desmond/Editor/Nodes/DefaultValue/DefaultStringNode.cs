using UnityEngine;
using System.Collections;

namespace Desmond { 

public class DefaultStringNode : DefaultPrimitiveNode {
    public string value = "";
    public override string primitiveToString() {
        return "\"" + value + "\"";
    }
}

}