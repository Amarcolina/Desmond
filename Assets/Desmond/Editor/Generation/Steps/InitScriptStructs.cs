using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class InitScriptStructs : GenerationStep {

    /* Generates all of the needed script structs, and initializes them 
     * with their default game objects
     */
    public override void doStep() {
        HashSet<GameObject> gameObjects = new HashSet<GameObject>();
        bool hasNull = false;

        LoadingBarUtil.beginChunk(nodes.Count, "", "Couting nodes : ", () => {
            foreach (Node node in nodes) {
                if (node.gameObjectInstance == null) {
                    hasNull = true;
                } else {
                    gameObjects.Add(node.gameObjectInstance);
                }
                
                LoadingBarUtil.recordProgress(node.ToString());
            }
        });
        
        foreach (GameObject gameObject in gameObjects) {
            ScriptStruct newScriptStruct = new ScriptStruct();
            newScriptStruct.scriptName = "TestScript";
            newScriptStruct.parentObject = gameObject;
            setScript(gameObject, newScriptStruct);
        }

        if (hasNull) {
            ScriptStruct newScriptStruct = new ScriptStruct();
            newScriptStruct.scriptName = "TestScript";
            newScriptStruct.parentObject = null;
            setScript(null, newScriptStruct);
        }

        LoadingBarUtil.beginChunk(nodes.Count, "", "Initializing nodes : ", () => {
            List<Node> nodesWeFound = new List<Node>();
            foreach (Node node in nodes) {
                ScriptStruct scriptStruct = getScript(node.gameObjectInstance);
                scriptStruct.nodes.Add(node);

                foreach(Element e in node.elements){
                    InputWithDefaultInfo input = e as InputWithDefaultInfo;
                    if (input != null && input.defaultValue != null) {
                        nodesWeFound.Add(input.defaultValue);
                        scriptStruct.nodes.Add(input.defaultValue);
                    }
                }

                LoadingBarUtil.recordProgress(node.ToString());
            }

            nodes.AddRange(nodesWeFound);
        });
    }
}

}