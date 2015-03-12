using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;   

namespace Desmond {

public class ObjectFieldStruct : FieldStruct {
    public Object value;

    public ObjectFieldStruct(ScriptElementKey key, string type, Object value)
        : base(key, type) {
        this.value = value;
    }
}

public class DefaultObjectNode : DefaultValueNode {
    public Object value;

    string fieldName = "defaultObject";

    public override List<FieldStruct> getFieldStructs() {
        List<FieldStruct> list = new List<FieldStruct>();

        ObjectFieldStruct s = new ObjectFieldStruct(new ScriptElementKey(this, fieldName), type, value);
        s.isPublic = true;
        list.Add(s);
        return list;
    }

    public override List<ExpressionMethodStruct> getExpressionStructs() {
        List<ExpressionMethodStruct> list = new List<ExpressionMethodStruct>();

        ExpressionMethodStruct s = new ExpressionMethodStruct(getKey(), fieldName, type);
        s.addCode("<" + fieldName + ">");
        s.inlineBehavior = InlineBehavior.FORCE_INLINE;
        list.Add(s);

        return list;
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
