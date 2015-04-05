using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond {

public class StringProperty : GenericPropertyValue<string> {
    public override void assignToProperty(SerializedProperty property) {
        property.stringValue = value;
    }
}

}

