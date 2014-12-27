using UnityEngine;
using System.Collections;

namespace Desmond {

    public class DefaultColorNode : DefaultPrimitiveNode {
        public Color value;
        public override string primitiveToString() {
            return "(new Color(" + value.r + "," + value.g + "," + value.b + "," + value.a + "))";
        }
    }

}
