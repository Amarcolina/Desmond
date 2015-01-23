using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

[System.Serializable]
public class CustomEventEditor {
    public const float BUTTON_WIDTH = 16;

    public Vector2 selectScrollPosition = Vector2.zero;
    public Vector2 editScrollPosition = Vector2.zero;
    public CustomEvent selected = null;

    public void drawEditor(Rect rect) {
        Rect eventSelectionRect = rect;
        eventSelectionRect.height *= 0.4f;

        Rect eventEditRect = rect;
        eventEditRect.y += eventSelectionRect.height;
        eventEditRect.height *= 0.6f;

        GUI.color = new Color(0.7f, 0.7f, 0.7f);
        GUI.Box(eventSelectionRect, "");
        GUI.Box(eventEditRect, "");
        GUI.color = Color.white;

        eventEditRect.width -= 2.0f;
        eventSelectionRect.width -= 2.0f;

        //Gui for drawing the event selection area
        GUILayout.BeginArea(eventSelectionRect);
        displaySelectorLayout();
        GUILayout.EndArea();
        
        //Gui for drawing the event edit area
        GUILayout.BeginArea(eventEditRect);
        if (selected != null) {
            displayEditorLayout();
        }
        GUILayout.EndArea();
    }

    private void displaySelectorLayout() {
        selectScrollPosition = GUILayout.BeginScrollView(selectScrollPosition);

        if (GUILayout.Button("Add New Custom Event")) {
            CustomEventHandler.addCustomEvent("NewCustomEvent");
        }

        CustomEvent eventToDelete = null;
        foreach (CustomEvent customEvent in CustomEventHandler.getCustomEvents()) {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(customEvent.customEventName, EditorStyles.label)) {
                selected = customEvent;
            }
            if (GUILayout.Button("X", GUILayout.MaxWidth(BUTTON_WIDTH))) {
                eventToDelete = customEvent;
            }
            GUILayout.EndHorizontal();
        }

        if (eventToDelete != null) {
            CustomEventHandler.deleteCustomEvent(eventToDelete);
        }

        GUILayout.EndScrollView();
    }

    private void displayEditorLayout() {
        GUILayout.Label("Custom Event Name");
        selected.customEventName = EditorGUILayout.TextField(selected.customEventName);
        GUILayout.Label("Custom Event Data");

        if (GUILayout.Button("Add New Input")) {
            selected.dataTypes.Add("int");
            selected.dataNames.Add("New Input");
            selected.dataDefaults.Add(null);
        }

        editScrollPosition = GUILayout.BeginScrollView(editScrollPosition);

        int inputToDelete = -1;
        for (int i = 0; i < selected.dataNames.Count; i++) {
            GUILayout.BeginHorizontal();
            selected.dataNames[i] = GUILayout.TextField(selected.dataNames[i]);
            EditorGUILayout.EnumPopup(KeyCode.A);
            if (GUILayout.Button("X", GUILayout.MaxWidth(BUTTON_WIDTH))) {
                inputToDelete = i;
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.ObjectField(null, typeof(GameObject));
        }

        if (inputToDelete != -1) {
            selected.dataTypes.RemoveAt(inputToDelete);
            selected.dataNames.RemoveAt(inputToDelete);
            selected.dataDefaults.RemoveAt(inputToDelete);
        }

        GUILayout.EndScrollView();
    }
}

}