using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public class InitExpressionMethods : GenerationStep {

    public override void doStep() {
        LoadingBarUtil.beginChunk(2, "", "", () => {
            LoadingBarUtil.beginChunk(nodes.Count, "", "Counting Expression Methods : ", () => {
                foreach (Node node in nodes) {
                    List<ExpressionMethodStruct> expressions = node.getExpressionStructs();
                    foreach (ExpressionMethodStruct expression in expressions) {
                        scripts[expression.structKey.parentNode.gameObjectInstance].methods[expression.structKey] = expression;
                    }
                    LoadingBarUtil.recordProgress(node.ToString());
                }
            });

            LoadingBarUtil.beginChunk(scripts.Count, "", "", () => {
                foreach (ScriptStruct script in scripts.Values) {
                    LoadingBarUtil.beginChunk(script.methods.Values.Count, "", "Initializing Expression Method : ", () => {
                        foreach (GenericMethodStruct method in script.methods.Values) {
                            forEveryExpressionLink(method, expression => {
                                expression.references++;
                                if (method.structKey.parentNode.gameObjectInstance != expression.structKey.parentNode.gameObjectInstance) {
                                    expression.isPublic = true;
                                }
                            });
                            LoadingBarUtil.recordProgress(method.ToString());
                        }
                    });
                }
            });
        });

    }
}

}
