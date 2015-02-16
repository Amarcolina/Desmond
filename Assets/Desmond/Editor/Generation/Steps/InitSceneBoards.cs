using UnityEngine;
using System.Collections;

namespace Desmond { 

public class InitSceneBoards : GenerationStep {

    public override void doStep() {
        DesmondSceneBase[] scriptBases = GameObject.FindObjectsOfType<DesmondSceneBase>();
        LoadingBarUtil.beginChunk(scriptBases.Length, "", "Cleaning up scene scripts : ", () => {
            foreach (DesmondSceneBase scriptBase in scriptBases) {
                GameObject.DestroyImmediate(scriptBase);
                LoadingBarUtil.recordProgress(scriptBase.name);
            }
        });

        boards = BoardHandler.getSceneBoards();
    }
}

}