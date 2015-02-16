using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public class InitFields : GenerationStep {

    public override void doStep() {
        LoadingBarUtil.beginChunk(2, "", "", () => {
            LoadingBarUtil.beginChunk(nodes.Count, "", "Counting Fields : ", () => {
                foreach (Node node in nodes) {
                    List<FieldStruct> fields = node.getFieldStructs();
                    foreach (FieldStruct field in fields) {
                        scripts[field.structKey.parentNode.gameObjectInstance].fields[field.structKey] = field;
                    }
                    LoadingBarUtil.recordProgress(node.ToString());
                }
            });

            LoadingBarUtil.beginChunk(scripts.Count, "", "", () => {
                foreach (ScriptStruct script in scripts.Values) {
                    LoadingBarUtil.beginChunk(script.methods.Values.Count, "", "Initializing Fields : ", () => {
                        foreach (GenericMethodStruct method in script.methods.Values) {
                            forEveryFieldLink(method, field => field.references++);
                            LoadingBarUtil.recordProgress(method.ToString());
                        }
                    });
                }
            });
        });
    }
}

}