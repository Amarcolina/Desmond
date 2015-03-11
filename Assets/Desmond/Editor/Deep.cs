using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public interface IDeepObject {
    IEnumerable ownedObjects();
}

public static class Deep {
    private static HashSet<Object> ownedObjects = new HashSet<Object>();

    public static void collectOwnedObjects(Object obj) {
        ownedObjects.Add(obj);
        IDeepObject deepObject = obj as IDeepObject;
        if (deepObject != null) {
            foreach (Object ownedObject in deepObject.ownedObjects()) {
                collectOwnedObjects(ownedObject);
            }
        }
    }

    public static Dictionary<Object, Object> copy(){
        Dictionary<Object, Object> oldToNew = new Dictionary<Object, Object>();

        LoadingBarUtil.beginChunk(2, "", "", () => {

            LoadingBarUtil.beginChunk(ownedObjects.Count, "", "Copying Objects...", () => {
                foreach (Object ownedObject in ownedObjects) {
                    LoadingBarUtil.recordProgress();

                    Object newObj = Object.Instantiate(ownedObject);
                    newObj.name = ownedObject.name;
                    oldToNew[ownedObject] = newObj;
                }
            });

            LoadingBarUtil.beginChunk(ownedObjects.Count, "", "Updating object references...", () => {
                foreach (Object newObject in oldToNew.Values) {
                    LoadingBarUtil.recordProgress();

                    SerializedObject sObj = new SerializedObject(newObject);
                    SerializedProperty it = sObj.GetIterator();

                    bool anyChanges = false;
                    while (it.Next(true)) {
                        if (it.propertyType == SerializedPropertyType.ObjectReference) {
                            Object reference = it.objectReferenceValue;
                            if (reference != null && oldToNew.ContainsKey(reference)) {
                                it.objectReferenceValue = oldToNew[it.objectReferenceValue];
                                anyChanges = true;
                            }
                        }
                    }

                    if (anyChanges) {
                        sObj.ApplyModifiedProperties();
                    }
                }
            });

            ownedObjects.Clear();
        });

        return oldToNew;
    }

    public static T copy<T>(T t) where T : Object {
        ownedObjects.Clear();
        collectOwnedObjects(t);
        Dictionary<Object, Object> objs = copy();
        return objs[t] as T;
    }

    public static void destroy(){
        foreach (Object obj in ownedObjects) {
            Object.DestroyImmediate(obj);
        }
        ownedObjects.Clear();
    }

    public static void destroy(Object obj) {
        ownedObjects.Clear();
        collectOwnedObjects(obj);
        destroy();
    }
}

}