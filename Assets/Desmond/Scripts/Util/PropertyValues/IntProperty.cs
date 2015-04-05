using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond {

public class IntProperty : GenericPropertyValue<int> {
    public override void assignToProperty(SerializedProperty property) {
        property.intValue = value;
    }
}

}