using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class UniqueStringMap {
    /*
    public HashSet<string> finalNames = new HashSet<string>();
    public Dictionary<NodeIdStruct, string> nodeIdToFinalName = new Dictionary<NodeIdStruct, string>();

    public string combineIdIndex(string s, int i) {
        return s + (i == 0 ? "" : "" + i);
    }

    //returns true if a new thing had to be created
    public bool getFinalName(Node node, string id, out string finalName) {
        if (nodeIdToFinalName.TryGetValue(new NodeIdStruct(node, id), out finalName)) {
            return false; ;
        }

        int index = 0;
        while (finalNames.Contains(combineIdIndex(id, index))) {
            index++;
        }

        finalName = combineIdIndex(id, index);
        finalNames.Add(finalName);
        nodeIdToFinalName[new NodeIdStruct(node, id)] = finalName;

        return true;
    }
     */
}

}