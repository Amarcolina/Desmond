using UnityEngine;
using UnityEditor;  
using System.Collections;

namespace Desmond { 

[System.Serializable]
public class SetObjectJob : SetPropertyJob {
    Object propertyValue;

    public void init(GameObject targetObject, string targetScriptName, string propertyName, Object propertyValue) {
        base.init(targetObject, targetScriptName, propertyName);
        this.propertyValue = propertyValue;
    }

    protected override void setProperty(SerializedProperty property) {
        property.objectReferenceValue = propertyValue;
    }
}

}