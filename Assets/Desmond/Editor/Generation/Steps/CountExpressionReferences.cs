using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public class CountExpressionReferences : GenerationStep {

    public override void doStep() {
        LoadingBarUtil.beginChunk(script.methods.Values.Count, "", "Initializing Expression Method : ", () => {
            foreach (GenericMethodStruct method in script.methods.Values.Where(m => m.shouldBeWritten())) {
                forEveryExpressionLink(method, expression => {
                    expression.references++;
                });
                LoadingBarUtil.recordProgress(method.ToString());
            }
        });

    }
}

}
