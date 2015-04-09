using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

[System.Serializable]
public class DefaultValueNode : Node, IDeepObject {
    public const string FIELD_NAME = "defaulObject";

    public PropertyValue propertyValue;

    public static DefaultValueNode getDefaultValueNode(string typeName) {
        DefaultValueNode node = ScriptableObject.CreateInstance<DefaultValueNode>();

        node.propertyValue = PropertyValue.createFromType(TypeUtil.searchForType(typeName));

        node.isVisible = false;
        node.hideFlags = HideFlags.HideInHierarchy;
        node.generateElements();
        return node;
    }

    public override void generateElements() {
        DataOutInfo dataOut = ScriptableObject.CreateInstance<DataOutInfo>();
        dataOut.init("out", propertyValue.fullTypeName, this);
        elements.Add(dataOut);
    }

    public override List<FieldStruct> getFieldStructs() {
        List<FieldStruct> list = new List<FieldStruct>();

        if (!propertyValue.hasStringRepresentation()) {
            FieldStruct s = new FieldStruct(new ScriptElementKey(this, FIELD_NAME), propertyValue.fullTypeName, propertyValue);
            list.Add(s);
        }
        
        return list;
    }

    public override List<ExpressionMethodStruct> getExpressionStructs() {
        List<ExpressionMethodStruct> list = new List<ExpressionMethodStruct>();

        ExpressionMethodStruct s = new ExpressionMethodStruct(getKey(), FIELD_NAME, propertyValue.fullTypeName);

        string representation;
        if (propertyValue.tryGetStringRepresentation(out representation)) {
            s.addCode(representation);
        } else {
            s.addCode("<" + FIELD_NAME + ">");
        }
        
        s.inlineBehavior = InlineBehavior.FORCE_INLINE;
        list.Add(s);

        return list;
    }

    public ScriptElementKey getKey() {
        return new ScriptElementKey(this, "out");
    }

    public IEnumerable<Object> getOwnedObjects() {
        yield return propertyValue;
    }
}

}