using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class BoardHandler {
    public const string ASSET_BOARDS_PATH = "Assets/Desmond/Boards";
    public const string ASSET_BOARD_FILTER = "t:DesmondBoard";

    private static SceneBoardHolder sceneBoardObject = null;
    private static List<DesmondBoard> prefabBoards = null;
    private static List<DesmondBoard> functionBoards = null;
    private static int statusKey = Random.Range(0, 1000000);

    public static SceneBoardHolder getSceneBoardHolder() {
        return sceneBoardObject;
    }

    public static int getBoardStatusKey() {
        return statusKey;
    }

    public static List<DesmondBoard> getSceneBoards() {
        ensureSceneBoardsLoaded();
        List<DesmondBoard> list = new List<DesmondBoard>();
        foreach (DesmondBoard board in sceneBoardObject.sceneBoards) {
            list.Add(board);
        }
        return list;
    }

    public static List<DesmondBoard> getAssetBoards() {
        ensureAssetBoardsLoaded();
        List<DesmondBoard> list = new List<DesmondBoard>();
        list.AddRange(prefabBoards);
        list.AddRange(functionBoards);
        return list;
    }

    public static List<DesmondBoard> getAllBoards() {
        ensureLoaded();
        List<DesmondBoard> boards = new List<DesmondBoard>();
        foreach (DesmondBoard board in sceneBoardObject.sceneBoards) {
            boards.Add(board);
        }
        boards.AddRange(prefabBoards);
        boards.AddRange(functionBoards);
        return boards;
    }

    public static void reloadBoards() {
        reloadAssetBoards();
        reloadSceneBoards();
    }

    public static void ensureLoaded() {
        ensureAssetBoardsLoaded();
        ensureSceneBoardsLoaded();
    }

    private static void ensureAssetBoardsLoaded() {
        if (prefabBoards == null || functionBoards == null) {
            reloadAssetBoards();
            return;
        }
        foreach (DesmondBoard board in prefabBoards) {
            if (board == null) {
                reloadAssetBoards();
                return;
            }
        }
        foreach (DesmondBoard board in functionBoards) {
            if (board == null) {
                reloadAssetBoards();
                return;
            }
        }
    }

    private static void reloadAssetBoards() {
        string[] folder = { ASSET_BOARDS_PATH };
        string[] guids = AssetDatabase.FindAssets(ASSET_BOARD_FILTER, folder);

        prefabBoards = new List<DesmondBoard>();
        functionBoards = new List<DesmondBoard>();

        foreach (string guid in guids) {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            DesmondBoard board = AssetDatabase.LoadAssetAtPath(path, typeof(DesmondBoard)) as DesmondBoard;
            if (board == null) {
                Debug.LogError("Could not open board at " + path);
            } else {
                board.assetPath = path;
            }

            if (board.boardType == DesmondBoardType.PREFAB_BOARD) {
                prefabBoards.Add(board);
            } else if(board.boardType == DesmondBoardType.FUNCTION_BOARD){
                prefabBoards.Add(board);
            } else {
                Debug.LogError("Unexpected board type: " + board.boardType + "\nBoard " + board + " was not loaded!");
            }
        }

        statusKey++;
    }

    private static void ensureSceneBoardsLoaded() {
        if (sceneBoardObject == null) {
            reloadSceneBoards();
            return;
        }
        foreach (DesmondBoard board in sceneBoardObject.sceneBoards) {
            if (board == null) {
                reloadSceneBoards();
                return;
            }
        }
    }

    private static void reloadSceneBoards() {
        SceneBoardHolder[] foundHolders = Object.FindObjectsOfType<SceneBoardHolder>();

        if (foundHolders.Length == 0) {
            Debug.Log("There was no scene board holder found, so one has been created");
            GameObject desmondObject = new GameObject("_DesmondScript");
            sceneBoardObject = desmondObject.AddComponent<SceneBoardHolder>();
            statusKey++;
        }

        if (foundHolders.Length >= 1) {
            if (sceneBoardObject != foundHolders[0]) {
                statusKey++;
            }
            sceneBoardObject = foundHolders[0];
        }

        if (foundHolders.Length > 1) {
            List<DesmondBoard> homelessBoards = new List<DesmondBoard>();
            for (int i = 1; i < foundHolders.Length; i++) {
                SceneBoardHolder extraHolder = foundHolders[i];
                foreach (DesmondBoard homelessBoard in extraHolder.sceneBoards) {
                    if (!homelessBoards.Contains(homelessBoard)) {
                        homelessBoards.Add(homelessBoard);
                    }
                }
                Debug.LogWarning("Extra SceneBoardHolder " + extraHolder + " found in scene\nIt has been removed and it's boards consolodated");

                GameObject.DestroyImmediate(extraHolder);
            }
            foreach (DesmondBoard homelessBoard in homelessBoards) {
                if (!sceneBoardObject.sceneBoards.Contains(homelessBoard)) {
                    sceneBoardObject.sceneBoards.Add(homelessBoard);
                }
            }
            statusKey++;
        }

        sceneBoardObject.sceneBoards.RemoveAll(board => board == null);
    }

    public static DesmondBoard createBoard(DesmondBoardType boardType, string boardName, string scriptName) {
        statusKey++;
        if (boardType == DesmondBoardType.SCENE_BOARD) {
            return createSceneBoard(boardName, scriptName);
        } else {
            return createAssetBoard(boardType, boardName, scriptName);
        }
    }

    private static DesmondBoard createAssetBoard(DesmondBoardType boardType, string boardName, string scriptName) {
        DesmondBoard newBoard = ScriptableObject.CreateInstance<DesmondBoard>();
        newBoard.name = boardName;
        newBoard.boardType = boardType;
        newBoard.scriptName = scriptName;

        AssetDatabase.CreateAsset(newBoard, ASSET_BOARDS_PATH + "/" + boardName + ".asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        reloadAssetBoards();

        return newBoard;
    }

    private static DesmondBoard createSceneBoard(string boardName, string scriptName) {
        ensureSceneBoardsLoaded();

        DesmondBoard newBoard = ScriptableObject.CreateInstance<DesmondBoard>();
        newBoard.name = boardName;
        newBoard.boardType = DesmondBoardType.SCENE_BOARD;
        newBoard.scriptName = scriptName;

        sceneBoardObject.sceneBoards.Add(newBoard);

        return newBoard;
    }

    public static void deleteBoard(DesmondBoard board) {
        if (board.boardType == DesmondBoardType.SCENE_BOARD) {
            ensureSceneBoardsLoaded();
            sceneBoardObject.sceneBoards.Remove(board);
            reloadSceneBoards();
        } else {
            ensureAssetBoardsLoaded();
            AssetDatabase.DeleteAsset(board.assetPath);
            reloadAssetBoards();
        }
        statusKey++;
    }

    public static void addAssetToCurrentBoard(Object obj) {
        if (obj != null) {
            DesmondBoard selectedBoard = BoardList.getSelectedBoard();
            if (selectedBoard != null) {

                //Only add asset to database if the board is an asset board
                //Fail silently if it is not
                if (selectedBoard.boardType != DesmondBoardType.SCENE_BOARD) {
                    ensureAssetBoardsLoaded();
                    AssetDatabase.AddObjectToAsset(obj, selectedBoard);
                }
            }
        }
    }
}

}