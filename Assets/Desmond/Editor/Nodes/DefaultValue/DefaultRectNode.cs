using UnityEngine;
using System.Collections;

namespace Desmond {

    public class DefaultRectNode : DefaultPrimitiveNode {
        public Rect value;
        public override string primitiveToString() {
            return "(new Rect(" + value.x + "," + value.y + "," + value.width + "," + value.height + "))";
        }
    }

}