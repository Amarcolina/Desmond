using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond {

[System.Serializable]
public class FloatProperty : GenericPropertyValue<float> {
    public override void applyTo(SerializedProperty property) {
        property.floatValue = value;
    }
}

}