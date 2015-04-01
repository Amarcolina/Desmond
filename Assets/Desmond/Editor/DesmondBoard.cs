using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond{

public enum DesmondBoardType {
    SCENE_BOARD,
    PREFAB_BOARD,
    FUNCTION_BOARD
}

public class DesmondBoard : ScriptableObject, IPathable, IDeepObject, IValidatable {
    [System.NonSerialized]
    public string assetPath;

    public DesmondBoardType boardType = DesmondBoardType.SCENE_BOARD;
    public List<Node> nodesInBoard = new List<Node>();
    public string scriptName;

    public List<PropertyValue> scriptProperties = new List<PropertyValue>();

    public class PropertyValue {
        public object propertyValue;
        public List<string> propertyPath;

        public PropertyValue(object propertyValue, params string[] propertyPath) {
            this.propertyValue = propertyValue;
            this.propertyPath = new List<string>(propertyPath);
        }

        public PropertyValue(object propertyValue, List<string> propertyPath) {
            this.propertyValue = propertyValue;
            this.propertyPath = new List<string>(propertyPath);
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

    public IEnumerable ownedObjects() {
        return (IEnumerable)nodesInBoard;
    }

    public bool validate() {
        if (nodesInBoard == null) {
            nodesInBoard = new List<Node>();
        }

        if (scriptName == null || scriptName == "") {
            scriptName = "DefaultScriptName";
        }

        for (int i = nodesInBoard.Count - 1; i >= 0; i--) {
            if (nodesInBoard[i] == null || !nodesInBoard[i].validate()) {
                Debug.LogWarning("Removing node " + nodesInBoard[i] + " because it failed to validate!");
                nodesInBoard.RemoveAt(i);
                continue;
            }
        }

        return true;
    }
}

}