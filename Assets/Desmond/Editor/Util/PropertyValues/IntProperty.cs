using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond {

public class IntProperty : GenericPropertyValue<int> {
    public override void applyTo(SerializedProperty property) {
        property.intValue = value;
    }
}

}