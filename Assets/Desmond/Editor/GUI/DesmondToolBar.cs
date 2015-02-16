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

    public List<PostCompilationJob> jobsToDo = null;

    private static bool wasJustReloaded = true;

    public void doTopBar(Rect rect) {
        if (wasJustReloaded) {
            doJobs();
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
                EditorGUI.BeginDisabledGroup(jobsToDo != null);
                if (GUILayout.Button("Generate All Scene Boards", EditorStyles.toolbarButton)) {
                    jobsToDo = BoardBuilder.buildSceneBoards();
                    if (!EditorApplication.isCompiling) {
                        doJobs();
                    }
                }
                EditorGUI.EndDisabledGroup();
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

    private void doJobs() {
        if (jobsToDo != null) {
            foreach (PostCompilationJob job in jobsToDo) {
                job.doJob();
                GameObject.DestroyImmediate(job);
            }
            jobsToDo = null;
        }
    }
}

}