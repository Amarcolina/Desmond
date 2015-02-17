using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public class FinalizeExpressionMethods : GenerationStep {

    public override void doStep() {
        LoadingBarUtil.beginChunk(scriptCount, "", "", () => {
            foreach (ScriptStruct script in scripts) {
                LoadingBarUtil.beginChunk(script.methods.Values.Count, "", "", () => {
                    foreach (GenericMethodStruct genericMethod in script.methods.Values) {
                        LoadingBarUtil.recordProgress("Finalizing expression method : " + genericMethod.ToString());
                        forEveryExpressionLink(genericMethod, linkedExpression => {
                            return calculateExpressionText(linkedExpression);
                        });
                    }
                });

            }
        });

    }

    public string calculateExpressionText(ExpressionMethodStruct expression) {
        if (!expression.shouldBeInlined()) {
            return expression.methodName + "()";
        }

        Debug.Log(":::::");
        foreach(string line in expression.codeBlock){
            Debug.Log(line);
        }
        Assert.equals(expression.codeBlock.Count, 1);

        //Make sure expression is completely parsed (done recursively) before inlining
        forEveryExpressionLink(expression, linkedExpression => calculateExpressionText(linkedExpression));

        return expression.codeBlock[0];
    }
}

}