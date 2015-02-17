using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public class AnimationCurveFieldStruct : FieldStruct {
    public AnimationCurve value;

    public AnimationCurveFieldStruct(ScriptElementKey key, string type, string name, AnimationCurve value) : base(key, type, name){
        this.value = value;
        this.defaultValue = value;
    }
}

public class DefaultAnimationCurveNode : DefaultValueNode {
    public AnimationCurve value;

    public override List<FieldStruct> getFieldStructs() {
        List<FieldStruct> list = new List<FieldStruct>();

        string name = "defaultCurve";

        AnimationCurveFieldStruct s = new AnimationCurveFieldStruct(new ScriptElementKey(this, name), type, name, value);
        s.isPublic = true;
        list.Add(s);
        return list;
    }

    public override List<ExpressionMethodStruct> getExpressionStructs() {
        List<ExpressionMethodStruct> list = new List<ExpressionMethodStruct>();

        ExpressionMethodStruct s = new ExpressionMethodStruct(getKey(), "defaultCurve", type);
        s.addCode("<defaultCurve>");
        s.inlineBehavior = InlineBehavior.FORCE_INLINE;
        list.Add(s);

        return list;
    }
}

}
