using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public class CountMethodReferences : GenerationStep {

    public override void doStep() {
        LoadingBarUtil.beginChunk(nodes.Count, "", "", () => {
            foreach (Node node in nodes) {
                List<CustomMethodStruct> customMethodStructs = node.getCustomMethodStructs();
                foreach (CustomMethodStruct customMethod in customMethodStructs) {
                    countMethodReferences(customMethod);
                }
                LoadingBarUtil.recordProgress(node.ToString());
            }
        });
    }

    private void countMethodReferences(GenericMethodStruct genericMethod) {
        genericMethod.references++;
        if (genericMethod.references > 1) {
            return;
        }

        scripts[genericMethod.structKey.parentNode.gameObjectInstance].methods[genericMethod.structKey] = genericMethod;

        forEveryMethodLink(genericMethod, subMethod => countMethodReferences(subMethod));
    }
}

}