using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public class CountMethodReferences : GenerationStep {

    public override void doStep() {
        LoadingBarUtil.beginChunk(script.methods.Count, "", "", () => {
            foreach (GenericMethodStruct m in script.methods.Values) {
                if (m is CustomMethodStruct || m is MessageMethodStruct) {
                    countMethodReferences(m);
                }
                LoadingBarUtil.recordProgress(m.ToString());
            }
        });
    }

    private void countMethodReferences(GenericMethodStruct genericMethod) {
        genericMethod.references++;
        if (genericMethod.references > 1) {
            return;
        }

        forEveryMethodLink(genericMethod, subMethod => countMethodReferences(subMethod));
    }
}

}