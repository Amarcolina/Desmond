using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond {

[System.Serializable]
public class IntProperty : GenericPropertyValue<int> {
    public override void applyTo(SerializedProperty property) {
        property.intValue = value;
    }

    public override bool tryGetStringRepresentation(out string representation) {
        representation = value.ToString();
        return true;
    }
}

}