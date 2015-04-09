using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

[System.Serializable]
public abstract class PropertyValue : ScriptableObject {
    public string fullTypeName;

    public abstract void applyTo(SerializedProperty property);

    public abstract void init(object obj);

    public static PropertyValue createFromValue(object value) {
        PropertyValue v = createFromType(value.GetType());
        v.init(value);
        return v;
    }

    public static PropertyValue createFromType(System.Type type) {
        PropertyValue v = null;
        if (typeof(UnityEngine.Object).IsAssignableFrom(type))  v = ScriptableObject.CreateInstance<ObjectProperty>();
        if (type == typeof(AnimationCurve)) v = ScriptableObject.CreateInstance<AnimationCurveProperty>();
        if (type == typeof(bool)) v = ScriptableObject.CreateInstance<BoolProperty>();
        if (type == typeof(Bounds)) v = ScriptableObject.CreateInstance<BoundsProperty>();
        if (type == typeof(Color)) v = ScriptableObject.CreateInstance<ColorProperty>();
        if (typeof(Enum).IsAssignableFrom(type)) v = ScriptableObject.CreateInstance<EnumProperty>();
        if (type == typeof(float)) v = ScriptableObject.CreateInstance<FloatProperty>();
        if (type == typeof(int)) v = ScriptableObject.CreateInstance<IntProperty>();
        if (type == typeof(Quaternion)) v = ScriptableObject.CreateInstance<QuaternionProperty>();
        if (type == typeof(Rect)) v = ScriptableObject.CreateInstance<RectProperty>();
        if (type == typeof(string)) v = ScriptableObject.CreateInstance<StringProperty>();
        if (type == typeof(Vector2)) v = StringProperty.CreateInstance<Vector2Property>();
        if (type == typeof(Vector3)) v = StringProperty.CreateInstance<Vector3Property>();
        if (type == typeof(Vector4)) v = StringProperty.CreateInstance<Vector4Property>();

        if (v == null) {
            throw new System.Exception("PropertyValue could not be created for type " + type.FullName);
        }

        v.fullTypeName = type.FullName;

        return v;
    }

    public abstract void drawDefaultPropertyEditor(Rect rect);
    public virtual void drawCustomPropertyEditor(Rect rect) {
        drawDefaultPropertyEditor(rect);
    }

    public abstract object getValue();

    public virtual bool tryGetStringRepresentation(out string representation) {
        representation = null;
        return false;
    }

    public bool hasStringRepresentation() {
        string temp;
        return tryGetStringRepresentation(out temp);
    }
}

[System.Serializable]
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

    public override object getValue() {
        return value;
    }
}

}