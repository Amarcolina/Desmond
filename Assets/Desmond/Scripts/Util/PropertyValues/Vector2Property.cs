using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond {

public class Vector2Property : GenericPropertyValue<Vector2> {
    public override void assignToProperty(SerializedProperty property) {
        property.vector2Value = value;
    }
}

}
