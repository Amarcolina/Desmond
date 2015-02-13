using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class RecordNamespaceImports : GenerationStep {

    /* Adds the namespace imports to the scripts based on the nodes they contain
     */
    public void doStep() {
        foreach (Node node in nodes) {
            ScriptStruct scriptStruct = scripts[node.gameObjectInstance];
            scriptStruct.namespaceImports.UnionWith(node.getNamespaceImports());
        }
    }
}

}
