using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public class CountExpressionReferences : GenerationStep {

    public override void doStep() {
        LoadingBarUtil.beginChunk(scripts.Count, "", "", () => {
            foreach (ScriptStruct script in scripts.Values) {
                LoadingBarUtil.beginChunk(script.methods.Values.Count, "", "Initializing Expression Method : ", () => {
                    foreach (GenericMethodStruct method in script.methods.Values.Where(m => m.shouldBeWritten())) {
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

    }
}

}
