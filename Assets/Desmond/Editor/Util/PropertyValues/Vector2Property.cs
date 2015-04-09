using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond {

[System.Serializable]
public class Vector2Property : GenericPropertyValue<Vector2> {
    public override void applyTo(SerializedProperty property) {
        property.vector2Value = value;
    }
}

}
