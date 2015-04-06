using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond {

public class EnumProperty : GenericPropertyValue<int> {
    public override void applyTo(SerializedProperty property) {
        property.enumValueIndex = value;
    }
}

}
