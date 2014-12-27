using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;   

namespace Desmond { 

public class DefaultObjectNode : DefaultValueNode {
    public Object value;

    public override List<FieldStruct> getFieldStructs() {
        List<FieldStruct> list = new List<FieldStruct>();

        string name = "default" + getValueType().Name;
        if (value != null) {
            name = StringHelper.toClassName(value.name, false);
        }

        ObjectFieldStruct s = new ObjectFieldStruct(getKey(), type, name, value);
        s.isPublic = true;
        list.Add(s);
        return list;
    }

    public override List<ExpressionMethodStruct> getExpressionStructs() {
        List<ExpressionMethodStruct> list = new List<ExpressionMethodStruct>();

        ExpressionMethodStruct s = new ExpressionMethodStruct(getKey(), "default", type);
        s.addCode("<out>");
        list.Add(s);

        return list;
    }

    public override List<MethodStruct> getMethodStructs() {
        return new List<MethodStruct>();
    }

    public override List<GenericCodeStruct> getGenericCodeStructs() {
        return new List<GenericCodeStruct>();
    }

    public override void drawDefaultProperty(Rect r) {
        if (defaultProperty == null) {
            defaultObject = new SerializedObject(this);
            defaultProperty = defaultObject.FindProperty("value");
        }
        defaultObject.Update();
        value = EditorGUI.ObjectField(r, value, getValueType(), true);
        defaultObject.ApplyModifiedProperties();
    }
}

}
