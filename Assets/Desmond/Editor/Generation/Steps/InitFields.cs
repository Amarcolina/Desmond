using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public class InitFields : GenerationStep {

    public override void doStep() {
        LoadingBarUtil.beginChunk(nodes.Count, "", "Counting Fields : ", () => {
            foreach (Node node in nodes) {
                List<FieldStruct> fields = node.getFieldStructs();
                foreach (FieldStruct field in fields) {
                    scripts[field.structKey.parentNode.gameObjectInstance].fields[field.structKey] = field;
                }
                LoadingBarUtil.recordProgress(node.ToString());
            }
        });
    }
}

}