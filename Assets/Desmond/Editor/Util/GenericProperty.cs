using UnityEngine;
using UnityEditor;  
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class GenericProperty : ScriptableObject, IDeepObject {
    public List<string> propertyPath;
    public PropertyValue propertyValue;

    public static GenericProperty create(object value, params string[] path) {
        GenericProperty genericProperty = ScriptableObject.CreateInstance<GenericProperty>();
        genericProperty.propertyPath = new List<string>(path);
        genericProperty.propertyValue = PropertyValue.createFromValue(value);
        return genericProperty;
    }

    public void applyTo(SerializedObject obj) {
        SerializedProperty property = obj.FindProperty(propertyPath[0]);
        Assert.that(property != null, "Could not find property " + propertyPath[0] + " in sertialized object " + obj);
        for (int i = 1; i < propertyPath.Count; i++) {
            property = property.FindPropertyRelative(propertyPath[i]);
            Assert.that(property != null, "Could not find relative property " + propertyPath[i] + " in serialized object " + obj);
        }
        propertyValue.applyTo(property);
    }

    public IEnumerable ownedObjects() {
        yield return propertyValue;
    }
}
    
}
