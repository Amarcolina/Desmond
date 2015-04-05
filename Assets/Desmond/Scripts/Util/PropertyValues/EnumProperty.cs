using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond {

public class EnumProperty : GenericPropertyValue<int> {
    public override void assignToProperty(SerializedProperty property) {
        property.enumValueIndex = value;
    }
}

}
