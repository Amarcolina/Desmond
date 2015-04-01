using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

namespace Desmond { 

public static class SerializedPropertyUtil {
    public static void setGenericValue(this SerializedProperty property, object value) {
        if (value == null || value is UnityEngine.Object) { property.objectReferenceValue = (UnityEngine.Object)value; return; }

        if (value is AnimationCurve) { property.animationCurveValue = (AnimationCurve)value; return; }
        if (value is bool) { property.boolValue = (bool)value; return; }
        if (value is Bounds) { property.boundsValue = (Bounds)value; return; }
        if (value is Color) { property.colorValue = (Color)value; return; }
        if (value is Enum) { property.intValue = (int)value; return; }
        if (value is float) { property.floatValue = (float)value; return; }
        if (value is int) { property.intValue = (int)value; return; }  
        if (value is Quaternion) { property.quaternionValue = (Quaternion)value; return; }
        if (value is Rect) { property.rectValue = (Rect)value; return; }
        if (value is string) { property.stringValue = (string)value; return; }
        if (value is Vector2) { property.vector2Value = (Vector2)value; return; }
        if (value is Vector3) { property.vector3Value = (Vector3)value; return; }
        if (value is Vector4) { property.vector4Value = (Vector4)value; return; }
    }
}

}