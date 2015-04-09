using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond {

[System.Serializable]
public class ObjectProperty : GenericPropertyValue<Object> {
    public override void applyTo(SerializedProperty property) {
        property.objectReferenceValue = value;
    }
}

}
