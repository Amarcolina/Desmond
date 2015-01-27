using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class InitCustomMethods {

    /* Takes the custom method structs from the nodes and adds them to the script structs
     * no parsing needs to be done right now
     */
    public void doStep(ref List<Node> nodes, ref Dictionary<GameObject, ScriptStruct> scripts) {
        foreach (Node node in nodes) {
            List<CustomMethodStruct> customMethodStructs = node.getCustomMethodStructs();
            scripts[node.gameObjectInstance].customMethods.addAll(customMethodStructs);
        }
    }
}

}
