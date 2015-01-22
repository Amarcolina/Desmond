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
    private CustomEventHolder customEventHolder;

    public void refreshCustomEventList() {
        customEventHolder = AssetDatabase.LoadAssetAtPath(ASSET_CUSTOM_EVENTS_PATH, typeof(CustomEventHolder)) as CustomEventHolder;

        if (customEventHolder == null) {
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<CustomEventHolder>(), ASSET_CUSTOM_EVENTS_PATH);
        }
    }

    public List<CustomEvent> getCustomEvents() {
        refreshCustomEventList();
        return customEventHolder.customEvents;
    }

    public void deleteCustomEvent(CustomEvent customEvent) {
        refreshCustomEventList();
        customEventHolder.customEvents.Remove(customEvent);
        Object.DestroyImmediate(customEvent, true);
    }

    public void addCustomEvent(CustomEvent newEvent){
        refreshCustomEventList();
        customEventHolder.customEvents.Add(newEvent);
        AssetDatabase.AddObjectToAsset(newEvent, customEventHolder);
    }
}

}