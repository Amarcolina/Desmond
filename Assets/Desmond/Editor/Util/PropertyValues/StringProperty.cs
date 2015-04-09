using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond {

[System.Serializable]
public class StringProperty : GenericPropertyValue<string> {
    public override void applyTo(SerializedProperty property) {
        property.stringValue = value;
    }
}

}

