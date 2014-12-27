using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public class DefaultPrimitiveNode : DefaultValueNode {

    public virtual string primitiveToString() {
        return "";
    }
    
    public override List<ExpressionMethodStruct> getExpressionStructs() {
        List<ExpressionMethodStruct> list = new List<ExpressionMethodStruct>();

        ScriptStructKey key = new ScriptStructKey(this, "out");
        ExpressionMethodStruct s = new ExpressionMethodStruct(key, "default", type);
        s.addCode(primitiveToString());
        list.Add(s);

        return list;
    }
}

}