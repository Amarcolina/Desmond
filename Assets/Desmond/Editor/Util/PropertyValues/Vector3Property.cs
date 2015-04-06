using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond {

public class Vector3Property : GenericPropertyValue<Vector3> {
    public override void applyTo(SerializedProperty property) {
        property.vector3Value = value;
    }
}

}
