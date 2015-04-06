using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond {

public class QuaternionProperty : GenericPropertyValue<Quaternion> {
    public override void applyTo(SerializedProperty property) {
        property.quaternionValue = value;
    }
}

}
