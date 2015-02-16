using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public class FinalizeMethods : GenerationStep {

    public override void doStep() {
        LoadingBarUtil.beginChunk(scripts.Count, "", "", () => {
            foreach (ScriptStruct script in scripts.Values) {
                LoadingBarUtil.beginChunk(script.methods.Values.Count, "", "", () => {
                    foreach (GenericMethodStruct genericMethod in script.methods.Values) {
                        LoadingBarUtil.recordProgress("Finalizing expression method : " + genericMethod.ToString());
                        forEveryMethodLink(genericMethod, linkedMethod => {
                            return calculateMethodLines(linkedMethod);
                        });
                    }
                });

            }
        });
    }

    public List<string> calculateMethodLines(MethodStruct method) {
        if (!method.shouldBeInlined()) {
            List<string> ret = new List<string>();
            ret.Add(method.methodName + "();");
            return ret;
        }

        //Make sure expression is completely parsed (done recursively) before inlining
        forEveryMethodLink(method, linkedMethod => calculateMethodLines(linkedMethod));

        return method.codeBlock;
    }
}

}