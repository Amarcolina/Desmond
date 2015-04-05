using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public abstract class PropertyValue : ScriptableObject {
    public abstract void assignToProperty(SerializedProperty property);

    public static PropertyValue getPropertyValue(object obj) {
        return null;
    }
}

public abstract class GenericPropertyValue<T> : PropertyValue {
    [SerializeField]
    protected T value;
}

}