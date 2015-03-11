using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

        List<DesmondBoard> sceneBoards = BoardHandler.getSceneBoards();
        LoadingBarUtil.beginChunk(2, "", "Copying boards...", () => {
            foreach (DesmondBoard sceneBoard in sceneBoards) {
                Deep.collectOwnedObjects(sceneBoard);
            }
            LoadingBarUtil.recordProgress("");

            Dictionary<Object, Object> originalToCopy = Deep.copy();
            LoadingBarUtil.recordProgress("");

            foreach (Object copiedObj in originalToCopy.Values) {
                DesmondBoard copiedBoard = copiedObj as DesmondBoard;
                if (copiedBoard != null) {
                    boards.Add(copiedBoard);
                }
            }

            foreach (DesmondBoard board in boards) {
                nodes.AddRange(board.nodesInBoard);
            }
        });
    }
}

}