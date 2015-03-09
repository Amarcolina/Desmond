using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

[System.Serializable]
public class BoardList : ISerializationCallbackReceiver{
    public DesmondBoard selected = null;

    private bool creatingNewBoard = false;

    private static BoardList instance = null;
    private static int boardStatusKey = 0;

    private string newBoardName = "";
    private string newScriptName = "";
    private DesmondBoardType newBoardType = DesmondBoardType.SCENE_BOARD;

    private HierarchicalList<DesmondBoard> _listDisplay = null;

    private void rebuildList() {
        _listDisplay = new HierarchicalList<DesmondBoard>(BoardHandler.getAllBoards(), false);
        if(selected != null && selected.validate()){
            _listDisplay.select(selected);
        }
    }

    public BoardList() {
        instance = this;
    }

    private void resetBoardSettings() {
        creatingNewBoard = false;
        newBoardName = "";
        newScriptName = "";
    }

    public void doList(Rect rect) {
        if (_listDisplay == null || boardStatusKey != BoardHandler.getBoardStatusKey()) {
            boardStatusKey = BoardHandler.getBoardStatusKey();
            rebuildList();
        }

        GUI.color = new Color(0.7f, 0.7f, 0.7f);
        GUI.Box(rect, "");
        GUI.color = Color.white;

        GUI.Label(new Rect(0, 0, rect.width, 16), "Boards:");
        if (GUI.Button(new Rect(rect.width / 2, 0, rect.width / 2, 16), "Create New...", EditorStyles.toolbarButton)) {
            creatingNewBoard = true;
        }

        GUI.BeginGroup(new Rect(0, 18, rect.width, rect.height - 18));

        if (creatingNewBoard) {
            GUILayout.BeginArea(new Rect(0, 0, rect.width, rect.height - 18));
            GUILayout.Label("Board Name:");
            string updatedBoardName = EditorGUILayout.TextField(newBoardName);
            if (updatedBoardName != newBoardName) {
                newBoardName = updatedBoardName;
                newScriptName = newBoardName;
            }

            GUILayout.Label("Script Name:");
            newScriptName = EditorGUILayout.TextField(newScriptName);
            newScriptName = StringHelper.toClassName(newScriptName);

            GUILayout.Label("Board Type:");
            newBoardType = (DesmondBoardType) EditorGUILayout.EnumPopup(newBoardType);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Create")) {
                BoardHandler.createBoard(newBoardType, newBoardName, newScriptName);
                resetBoardSettings();
            }
            if (GUILayout.Button("Cancel")) {
                resetBoardSettings();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        } else {
            _listDisplay.drawList(new Rect(0, 0, rect.width, rect.height - 18));
        }
        
        GUI.EndGroup();
    }

    /* Board can only change state when a layout event is happening
     * That way the state remains constant throughout repains and such
     */
    public static DesmondBoard getSelectedBoard() {
        if (instance._listDisplay != null) {
            if (instance._listDisplay.highlighted != null) {
                if (Event.current.type == EventType.Layout) {
                    DesmondBoard boardToSelect = instance._listDisplay.highlighted.data;
                    if (boardToSelect != instance.selected && boardToSelect.validate()) {
                        instance.selected = boardToSelect;
                    }
                }
            }
        }
        return instance.selected;
    }

    public virtual void OnBeforeSerialize() {
        if (_listDisplay != null) {
            if (_listDisplay.highlighted != null) {
                selected = _listDisplay.highlighted.data;
            }
        }
    }

    public virtual void OnAfterDeserialize() {
        instance = this;
    }
}

}