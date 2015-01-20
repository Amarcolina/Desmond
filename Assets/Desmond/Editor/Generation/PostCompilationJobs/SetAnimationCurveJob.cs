using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond {

[System.Serializable]
public class SetAnimationCurveJob : SetPropertyJob {
    AnimationCurve propertyValue;

    public void init(GameObject targetObject, string targetScriptName, string propertyName, AnimationCurve propertyValue) {
        base.init(targetObject, targetScriptName, propertyName);
        this.propertyValue = propertyValue;
    }

    protected override void setProperty(SerializedProperty property) {
        property.animationCurveValue = propertyValue;
    }
}

}