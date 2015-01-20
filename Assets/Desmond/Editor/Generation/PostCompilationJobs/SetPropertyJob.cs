using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond {

[System.Serializable]
public class SetPropertyJob : PostCompilationJob {
    GameObject targetObject;
    string targetScriptName;
    string propertyName;

    protected virtual void init(GameObject targetObject, string targetScriptName, string propertyName) {
        this.targetObject = targetObject;
        this.targetScriptName = targetScriptName;
        this.propertyName = propertyName;
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
                setProperty(targetProperty);
                serializedObject.ApplyModifiedProperties();
                return;
            }
        }
        Debug.LogError("Property set failed!");
        Debug.LogError(targetObject + " : " + targetScriptName + " : " + propertyName);
    }

    protected virtual void setProperty(SerializedProperty property) {
    }
}

}
