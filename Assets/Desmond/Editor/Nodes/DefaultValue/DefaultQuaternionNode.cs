using UnityEngine;
using System.Collections;

namespace Desmond {

    public class DefaultQuaternionNode : DefaultPrimitiveNode {
        public Quaternion value = Quaternion.identity;
        public override string primitiveToString() {
            return "(new Quaternion(" + value.x + "," + value.y + "," + value.z + "," + value.w + "))";
        }
    }

}