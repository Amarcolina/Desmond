using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond {

public class QuaternionProperty : GenericPropertyValue<Quaternion> {
    public override void assignToProperty(SerializedProperty property) {
        property.quaternionValue = value;
    }
}

}
