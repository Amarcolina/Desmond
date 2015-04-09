using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public abstract class PropertyValue : ScriptableObject {
    public abstract void applyTo(SerializedProperty property);

    public abstract void init(object obj);

    public static PropertyValue create(object value) {
        PropertyValue v = null;
        if (value == null || value is UnityEngine.Object) v = ScriptableObject.CreateInstance<ObjectProperty>();

        if (value is AnimationCurve) v = ScriptableObject.CreateInstance<AnimationCurveProperty>();
        if (value is bool) v = ScriptableObject.CreateInstance<BoolProperty>();
        if (value is Bounds) v = ScriptableObject.CreateInstance<BoundsProperty>();
        if (value is Color) v = ScriptableObject.CreateInstance<ColorProperty>();
        if (value is Enum) v = ScriptableObject.CreateInstance<EnumProperty>();
        if (value is float) v = ScriptableObject.CreateInstance<FloatProperty>();
        if (value is int) v = ScriptableObject.CreateInstance<IntProperty>();
        if (value is Quaternion) v = ScriptableObject.CreateInstance<QuaternionProperty>();
        if (value is Rect) v = ScriptableObject.CreateInstance<RectProperty>();
        if (value is string) v = ScriptableObject.CreateInstance<StringProperty>();
        if (value is Vector2) v = StringProperty.CreateInstance<Vector2Property>();
        if (value is Vector3) v = StringProperty.CreateInstance<Vector3Property>();
        if (value is Vector4) v = StringProperty.CreateInstance<Vector4Property>();

        v.init(value);
        return v;
    }

    public abstract void drawDefaultPropertyEditor(Rect rect);
    public virtual void drawCustomPropertyEditor(Rect rect) {
        drawDefaultPropertyEditor(rect);
    }
}

public abstract class GenericPropertyValue<T> : PropertyValue {
    [SerializeField]
    protected T value;

    protected SerializedObject _mySerializedObject = null;
    protected SerializedProperty _serializedValue = null;

    protected SerializedObject serializedObject {
        get {
            if (_mySerializedObject == null) {
                _mySerializedObject = new SerializedObject(this);
            }
            return _mySerializedObject;
        }
    }

    protected SerializedProperty serializedValue {
        get {
            if (_serializedValue == null) {
                _serializedValue = serializedObject.FindProperty("value");
            }
            return _serializedValue;
        }
    }

    public override void init(object obj) {
        value = (T)obj;
    }

    public override void drawDefaultPropertyEditor(Rect rect) {
        serializedObject.UpdateIfDirtyOrScript();
        EditorGUI.PropertyField(rect, serializedValue, GUIContent.none);
        serializedObject.ApplyModifiedProperties();
    }
}

}