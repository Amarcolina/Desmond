using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class RecordNamespaceImports : GenerationStep {

    /* Adds the namespace imports to the scripts based on the nodes they contain
     */
    public override void doStep() {
        LoadingBarUtil.beginChunk(nodes.Count, "", "Recording namespace imports", () => {
            foreach (Node node in nodes) {
                script.namespaceImports.UnionWith(node.getNamespaceImports());
                LoadingBarUtil.recordProgress(node.ToString());
            }
        });
    }
}

}
