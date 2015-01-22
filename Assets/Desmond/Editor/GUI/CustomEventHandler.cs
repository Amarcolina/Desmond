using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Desmond{

public class CustomEventHolder : ScriptableObject{
    public List<CustomEvent> customEvents = new List<CustomEvent>();
}

public class CustomEventHandler {
    public const string ASSET_CUSTOM_EVENTS_PATH = "Assets/Desmond/Data/CustomEvents";
    private static CustomEventHolder customEventHolder;

    public static void refreshCustomEventList() {
        customEventHolder = AssetDatabase.LoadAssetAtPath(ASSET_CUSTOM_EVENTS_PATH, typeof(CustomEventHolder)) as CustomEventHolder;

        if (customEventHolder == null) {
            customEventHolder = ScriptableObject.CreateInstance<CustomEventHolder>();
            AssetDatabase.CreateAsset(customEventHolder, ASSET_CUSTOM_EVENTS_PATH);
        }
    }

    public static List<CustomEvent> getCustomEvents() {
        refreshCustomEventList();
        return customEventHolder.customEvents;
    }

    public static void deleteCustomEvent(CustomEvent customEvent) {
        refreshCustomEventList();
        customEventHolder.customEvents.Remove(customEvent);
        Object.DestroyImmediate(customEvent, true);
    }

    public static void addCustomEvent(string customEventName) {
        CustomEvent newEvent = ScriptableObject.CreateInstance<CustomEvent>();
        newEvent.customEventName = customEventName;

        refreshCustomEventList();
        customEventHolder.customEvents.Add(newEvent);
        AssetDatabase.AddObjectToAsset(newEvent, customEventHolder);
    }
}

}