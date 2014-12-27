using UnityEngine;
using UnityEditor;  
using System.Collections;

namespace Desmond { 

[System.Serializable]
public class SetObjectProperty : PostCompilationJob {
    GameObject targetObject;
    string targetScriptName;
    string propertyName;
    Object propertyValue;

    public void init(GameObject targetObject, string targetScriptName, string propertyName, Object propertyValue) {
        this.targetObject = targetObject;
        this.targetScriptName = targetScriptName;
        this.propertyName = propertyName;
        this.propertyValue = propertyValue;
    }

    public override int getPriority() {
        return 25;
    }

    public override void doJob() {
        MonoBehaviour[] scripts = targetObject.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts) {
            if (script.GetType().Name == targetScriptName) {
                SerializedObject serializedObject = new SerializedObject(script);
                SerializedProperty targetProperty = serializedObject.FindProperty(propertyName);
                targetProperty.objectReferenceValue = propertyValue;
                serializedObject.ApplyModifiedProperties();
                return;
            }
        }
        Debug.LogError("Property set failed!");
        Debug.LogError(targetObject + " : " + targetScriptName + " : " + propertyName + " : " + propertyValue);
    }
}

}