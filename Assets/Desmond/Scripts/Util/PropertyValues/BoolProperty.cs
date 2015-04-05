using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond {

public class BoolProperty : GenericPropertyValue<bool> {
    public override void assignToProperty(SerializedProperty property) {
        property.boolValue = value;
    }
}

}