using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond {

public class ObjectProperty : GenericPropertyValue<Object> {
    public override void applyTo(SerializedProperty property) {
        property.objectReferenceValue = value;
    }
}

}
