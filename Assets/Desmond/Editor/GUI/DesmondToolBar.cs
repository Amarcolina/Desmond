using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public enum ScriptBuildState {
    NONE,
    WAITING_FOR_COMPILE_TO_START,
    WAITING_FOR_COMPILE_TO_FINISH
}

[System.Serializable]
public class DesmondToolbar {
    public const float height = 18;
    public ScriptBuildState buildStatus = ScriptBuildState.NONE;

    private static bool wasJustReloaded = true;

    public void doTopBar(Rect rect) {
        if (wasJustReloaded) {
            wasJustReloaded = false;
        }

        GUILayout.BeginArea(rect);
        GUILayout.BeginHorizontal(GUILayout.MaxWidth(2));
        if (GUILayout.Button("Save", EditorStyles.toolbarButton)) {
            AssetDatabase.SaveAssets();
        }

        DesmondBoard board = BoardList.getSelectedBoard();

        if (board != null) {
            if (board.boardType == DesmondBoardType.SCENE_BOARD) {
                if (GUILayout.Button("Generate All Scene Boards", EditorStyles.toolbarButton)) {
                    BoardBuilder.buildSceneBoards();
                }
            } else if(board.boardType == DesmondBoardType.PREFAB_BOARD){
                if (GUILayout.Button("Generate Prefab Script", EditorStyles.toolbarButton)) {
                    Debug.LogWarning("Not implemented right now!");
                    //BoardBuilder.buildBoards(board);
                }
            }
        }

        if (GUILayout.Button("Reload All Definitions", EditorStyles.toolbarButton)) {
            BoardHandler.reloadBoards();
        }

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
}

}