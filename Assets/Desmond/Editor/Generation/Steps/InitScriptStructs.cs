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
        foreach (Node node in nodes) {
            gameObjects.Add(node.gameObjectInstance);
        }

        foreach (GameObject gameObject in gameObjects) {
            ScriptStruct newScriptStruct = new ScriptStruct();
            newScriptStruct.parentObject = gameObject;
            scripts[gameObject] = newScriptStruct;
        }

        foreach (Node node in nodes) {
            ScriptStruct scriptStruct = scripts[node.gameObjectInstance];
            scriptStruct.nodes.Add(node);
        }
    }
}

}