using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond {

public class FloatProperty : GenericPropertyValue<float> {
    public override void applyTo(SerializedProperty property) {
        property.floatValue = value;
    }
}

}