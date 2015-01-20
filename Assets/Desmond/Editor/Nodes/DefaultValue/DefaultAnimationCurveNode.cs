﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public class AnimationCurveFieldStruct : FieldStruct {
    public AnimationCurve value;

    public AnimationCurveFieldStruct(ScriptStructKey key, string type, string name, AnimationCurve value)
        : base(key, type, name, "") {
        this.value = value;
    }
}

public class DefaultAnimationCurveNode : DefaultValueNode {
    public AnimationCurve value;

    public override List<FieldStruct> getFieldStructs() {
        List<FieldStruct> list = new List<FieldStruct>();

        string name = "DefaultCurve";

        AnimationCurveFieldStruct s = new AnimationCurveFieldStruct(getKey(), type, name, value);
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
}

}
