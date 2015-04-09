using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class InitScriptStruct : GenerationStep {

    /* Generates all of the needed script structs, and initializes them 
     * with their default game objects
     */
    public override void doStep() {
        script = new ScriptStruct();
        script.scriptName = "TestScript";
        script.nodes.AddRange(nodes);

        List<Node> defaultValueNodes = new List<Node>();
        foreach (Node node in nodes) {
            foreach (Element e in node.elements) {
                InputWithDefaultInfo defaultInfo = e as InputWithDefaultInfo;
                if (defaultInfo != null && defaultInfo.defaultValue != null) {
                    defaultValueNodes.Add(defaultInfo.defaultValue);
                }
            }
            
        }
        nodes.AddRange(defaultValueNodes);
    }
}

}