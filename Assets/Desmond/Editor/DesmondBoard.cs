using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond{

public enum DesmondBoardType {
    SCENE_BOARD,
    PREFAB_BOARD,
    FUNCTION_BOARD
}

public class DesmondBoard : ScriptableObject, IPathable {
    [System.NonSerialized]
    public string assetPath;

    public DesmondBoardType boardType = DesmondBoardType.SCENE_BOARD;
    public List<Node> nodesInBoard = new List<Node>();
    public string scriptName;

    public void OnDestroy() {
        foreach(Node node in nodesInBoard){
            if (node != null) {
                DestroyImmediate(node);
            }
        }
    }

    public string getPath() {
        switch (boardType) {
            case DesmondBoardType.SCENE_BOARD:
                return "1. Scene/" + name;
            case DesmondBoardType.PREFAB_BOARD:
                return "2. Prefab/" + name;
            case DesmondBoardType.FUNCTION_BOARD:
                return "3. Function/" + name;
            default:
                return "ERROR";
        }
    }
}

}