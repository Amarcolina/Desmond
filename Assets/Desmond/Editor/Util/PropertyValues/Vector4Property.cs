using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond {

public class Vector4Property : GenericPropertyValue<Vector4> {
    public override void applyTo(SerializedProperty property) {
        property.vector4Value = value;
    }
}

}
