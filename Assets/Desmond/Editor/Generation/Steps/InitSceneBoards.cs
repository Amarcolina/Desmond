using UnityEngine;
using System.Collections;

namespace Desmond { 

public class InitSceneBoards : GenerationStep {

    public override void doStep() {
        DesmondSceneBase[] scriptBases = GameObject.FindObjectsOfType<DesmondSceneBase>();
        foreach (DesmondSceneBase scriptBase in scriptBases) {
            GameObject.DestroyImmediate(scriptBase);
        }

        boards = BoardHandler.getSceneBoards();
    }
}

}