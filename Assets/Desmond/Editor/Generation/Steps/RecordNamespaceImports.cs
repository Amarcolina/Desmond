using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class RecordNamespaceImports : MonoBehaviour {

    /* Adds the namespace imports to the scripts based on the nodes they contain
     */
    public void doStep(ref List<Node> nodes, ref Dictionary<GameObject, ScriptStruct> scripts) {
        foreach (Node node in nodes) {
            ScriptStruct scriptStruct = scripts[node.gameObjectInstance];
            scriptStruct.namespaceImports.UnionWith(node.getNamespaceImports());
        }
    }
}

}
