﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class InitMethodStructs : GenerationStep{
    public override void doStep() {
        foreach (Node node in nodes) {
            List<CustomMethodStruct> customMethodStructs = node.getCustomMethodStructs();
            foreach (CustomMethodStruct customMethod in customMethodStructs) {
                addMethod(customMethod);
            }
        }
    }

    private void addMethod(GenericMethodStruct genericMethod) {
        genericMethod.references++;
        if (genericMethod.references > 1) {
            return;
        }

        scripts[genericMethod.structKey.parentNode.gameObjectInstance].methods[genericMethod.structKey] = genericMethod;

        forEveryMethodLink(genericMethod, subMethod => addMethod(subMethod));
    }
}

}