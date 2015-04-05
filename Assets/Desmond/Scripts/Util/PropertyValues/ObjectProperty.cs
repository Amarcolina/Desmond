using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond {

public class ObjectProperty : GenericPropertyValue<Object> {
    public override void assignToProperty(SerializedProperty property) {
        property.objectReferenceValue = value;
    }
}

}
