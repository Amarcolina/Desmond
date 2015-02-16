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
        LoadingBarUtil.beginChunk(nodes.Count, "", "Couting nodes : ", () => {
            foreach (Node node in nodes) {
                if (node.gameObjectInstance == null) {
                    node.gameObjectInstance = BoardHandler.getSceneBoardHolder().gameObject;
                }
                gameObjects.Add(node.gameObjectInstance);
                LoadingBarUtil.recordProgress(node.ToString());
            }
        });
        
        foreach (GameObject gameObject in gameObjects) {
            ScriptStruct newScriptStruct = new ScriptStruct();
            newScriptStruct.scriptName = "TestScript";
            newScriptStruct.parentObject = gameObject;
            scripts[gameObject] = newScriptStruct;
        }

        LoadingBarUtil.beginChunk(nodes.Count, "", "Initializing nodes : ", () => {
            foreach (Node node in nodes) {
                ScriptStruct scriptStruct = scripts[node.gameObjectInstance];
                scriptStruct.nodes.Add(node);
                LoadingBarUtil.recordProgress(node.ToString());
            }
        });

        Debug.Log(scripts.Count);
    }
}

}