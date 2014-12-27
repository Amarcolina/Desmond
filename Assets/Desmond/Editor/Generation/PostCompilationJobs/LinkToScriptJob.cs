using UnityEngine;
using UnityEditor;  
using System.Collections;

namespace Desmond { 

public class LinkToScriptJob : PostCompilationJob {
    GameObject targetObject;
    string targetScriptName;
    string propertyName;
    GameObject linkedObject;
    string linkedScriptName;

    public void init(GameObject targetObject, string targetScriptName, string propertyName, GameObject linkedObject, string linkedScriptName) {
        this.targetObject = targetObject;
        this.targetScriptName = targetScriptName;
        this.propertyName = propertyName;
        this.linkedObject = linkedObject;
        this.linkedScriptName = linkedScriptName;

    }
    public override int getPriority() {
        return 25;
    }

    public override void doJob() {
        MonoBehaviour targetScript = (MonoBehaviour) targetObject.GetComponent(targetScriptName);
        if (targetScript != null) {
            Debug.Log(1);
            MonoBehaviour linkedScript = (MonoBehaviour)linkedObject.GetComponent(linkedScriptName);
            if (linkedScript != null) {
                Debug.Log(2);
                SerializedObject serializedObject = new SerializedObject(targetScript);
                SerializedProperty targetProperty = serializedObject.FindProperty(propertyName);
                if (targetProperty != null) {
                    targetProperty.objectReferenceValue = linkedScript;
                    serializedObject.ApplyModifiedProperties();
                    return;
                }
            }
        }
        Debug.LogError("Property set failed!");
        Debug.LogError(targetObject + " : " + targetScriptName + " : " + propertyName + " : " + linkedObject + " : " + linkedScriptName);
    }
}

}