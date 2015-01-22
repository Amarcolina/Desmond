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
        eventSelectionRect.height /= 2;

        Rect eventEditRect = eventSelectionRect;
        eventEditRect.y += eventEditRect.height;

        CustomEvent eventToDelete = null;

        //Gui for drawing the event selection area
        GUILayout.BeginArea(eventSelectionRect);
        GUILayout.BeginScrollView(selectScrollPosition);

        foreach (CustomEvent customEvent in CustomEventHandler.getCustomEvents()) {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(customEvent.customEventName, EditorStyles.label, GUILayout.MaxWidth(rect.width - BUTTON_WIDTH))) {
                selected = customEvent;
            }
            if (GUILayout.Button("-")) {
                eventToDelete = customEvent;
            }
            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add New Custom Event")) {
            CustomEventHandler.addCustomEvent("NewCustomEvent");
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();

        if (eventToDelete != null) {
            CustomEventHandler.deleteCustomEvent(eventToDelete);
        }
        
        //Gui for drawing the event edit area
        GUILayout.BeginArea(eventEditRect);
        GUILayout.BeginScrollView(editScrollPosition);

        selected.customEventName = EditorGUILayout.TextField("Custom Event Name", selected.customEventName);
        GUILayout.Label("Custom Event Data");

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

        if (GUILayout.Button("Add New Input")) {
            selected.dataTypes.Add("int");
            selected.dataNames.Add("New Input");
            selected.dataDefaults.Add(null);
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }
}

}