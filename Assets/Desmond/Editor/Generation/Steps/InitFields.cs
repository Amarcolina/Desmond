using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public class InitFields : GenerationStep {

    public override void doStep() {
        //Add all fields to scripts
        foreach (Node node in nodes) {
            List<FieldStruct> fields = node.getFieldStructs();
            foreach (FieldStruct field in fields) {
                scripts[field.structKey.parentNode.gameObjectInstance].fields[field.structKey] = field;
            }
        }

        foreach (ScriptStruct script in scripts.Values) {
            foreach (GenericMethodStruct method in script.methods.Values) {
                forEveryFieldLink(method, field => field.references++);
            }
        }
    }
}

}