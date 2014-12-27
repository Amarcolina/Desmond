using UnityEngine;
using System.Collections;

namespace Desmond {

    public class DefaultVector3Node : DefaultPrimitiveNode {
        public Vector3 value = Vector2.zero;
        public override string primitiveToString() {
            return "(new Vector3(" + value.x + "," + value.y + "," + value.z + "))";
        }
    }

}