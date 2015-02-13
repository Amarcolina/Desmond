using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public class FinalizeExpressionMethods : GenerationStep {

    public override void doStep() {
        foreach (ScriptStruct script in scripts.Values) {
            foreach (GenericMethodStruct genericMethod in script.methods.Values) {
                forEveryExpressionLink(genericMethod, linkedExpression => {
                    return calculateExpressionText(linkedExpression);
                });
            }
        }
    }

    public string calculateExpressionText(ExpressionMethodStruct expression) {
        if (expression.shouldBeInlined()) {
            return expression.methodName + "()";
        }

        Assert.equals(expression.codeBlock.Count, 1);

        //Make sure expression is completely parsed (done recursively) before inlining
        forEveryExpressionLink(expression, linkedExpression => calculateExpressionText(linkedExpression));  

        return expression.codeBlock[0];
    }
}

}