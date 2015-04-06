using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond {

public class BoolProperty : GenericPropertyValue<bool> {
    public override void applyTo(SerializedProperty property) {
        property.boolValue = value;
    }
}

}