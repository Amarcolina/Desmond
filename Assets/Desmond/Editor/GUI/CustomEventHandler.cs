using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Desmond{

public class CustomEventHandler {
    public const string ASSET_CUSTOM_EVENTS_PATH = "Assets/Desmond/Data/CustomEvents.asset";
    private static CustomEventHolder customEventHolder;

    public static void refreshCustomEventList() {
        customEventHolder = AssetDatabase.LoadAssetAtPath(ASSET_CUSTOM_EVENTS_PATH, typeof(CustomEventHolder)) as CustomEventHolder;

        if (customEventHolder == null) {
            customEventHolder = ScriptableObject.CreateInstance<CustomEventHolder>();
            AssetDatabase.CreateAsset(customEventHolder, ASSET_CUSTOM_EVENTS_PATH);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    public static List<CustomEvent> getCustomEvents() {
        if (customEventHolder == null) {
            refreshCustomEventList();
        }
        
        return customEventHolder.customEvents;
    }

    public static void deleteCustomEvent(CustomEvent customEvent) {
        if (customEventHolder == null) {
            refreshCustomEventList();
        }

        customEventHolder.customEvents.Remove(customEvent);
        Object.DestroyImmediate(customEvent, true);
    }

    public static void addCustomEvent(string customEventName) {
        if (customEventHolder == null) {
            refreshCustomEventList();
        }

        CustomEvent newEvent = ScriptableObject.CreateInstance<CustomEvent>();
        newEvent.customEventName = customEventName;
        
        customEventHolder.customEvents.Add(newEvent);
        AssetDatabase.AddObjectToAsset(newEvent, customEventHolder);
    }
}

}