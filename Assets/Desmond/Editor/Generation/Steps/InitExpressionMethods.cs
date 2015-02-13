using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class InitExpressionMethods : GenerationStep {

    public override void doStep() {
        foreach (Node node in nodes) {
            List<ExpressionMethodStruct> expressions = node.getExpressionStructs();
            foreach (ExpressionMethodStruct expression in expressions) {
                scripts[expression.structKey.parentNode.gameObjectInstance].methods[expression.structKey] = expression;
            }
        }

        foreach (ScriptStruct script in scripts.Values) {
            foreach (GenericMethodStruct method in script.methods.Values) {
                forEveryExpressionLink(method, expression => {
                    expression.references++;
                    if (method.structKey.parentNode.gameObjectInstance != expression.structKey.parentNode.gameObjectInstance) {
                        expression.isPublic = true;
                    }
                });
            }
        }
    }
}

}
