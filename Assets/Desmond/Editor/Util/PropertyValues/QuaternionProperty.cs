using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond {

[System.Serializable]
public class QuaternionProperty : GenericPropertyValue<Quaternion> {
    public override void applyTo(SerializedProperty property) {
        property.quaternionValue = value;
    }
}

}
