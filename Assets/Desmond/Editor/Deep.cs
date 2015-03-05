using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public interface IDeepObject {
}

public static class Deep {

    public static T copy<T>(T obj) where T : Object{
        return copySurface(obj, -1);
    }

    public static T copySurface<T>(T obj, int levelsDeep = 1) where T : Object {
        Assert.that(obj is IDeepObject);
        System.Type objType = obj.GetType();

        Dictionary<T, T> oldToNew = new Dictionary<T, T>();
        T newObj = copyInternal(obj, objType, oldToNew, levelsDeep);
        return newObj;
    }

    private static T copyInternal<T>(T obj, System.Type objType, Dictionary<T, T> oldToNew, int levelsDown) where T : Object{
        if (oldToNew.ContainsKey(obj)) {
            return oldToNew[obj];
        }

        T newObj = Object.Instantiate(obj) as T;
        oldToNew[obj] = newObj;

        if (levelsDown != 0) {
            SerializedObject sObj = new SerializedObject(newObj);
            SerializedProperty it = sObj.GetIterator();

            while (it.Next(true)) {
                if (it.propertyType == SerializedPropertyType.ObjectReference) {
                    Debug.Log(it.name);
                    Object reference = it.objectReferenceValue;
                    if (reference != null && reference.GetType() == objType) {
                        it.objectReferenceValue = copyInternal(it.objectReferenceValue as T, objType, oldToNew, levelsDown - 1);
                    }
                }
            }

            sObj.ApplyModifiedProperties();
        }
        
        return newObj;
    }

    public static void destroy<T>(T obj) where T : Object{
        destroySurface(obj, -1);
    }

    public static void destroySurface<T>(T obj, int levelsDeep = 1) where T : Object{
        Assert.that(obj is IDeepObject);
        System.Type objType = obj.GetType();

        HashSet<T> destroyedObjects = new HashSet<T>();
        destroyInternal(obj, objType, destroyedObjects, levelsDeep);
    }

    private static void destroyInternal<T>(T obj, System.Type objType, HashSet<T> destroyedObjects, int levelsDown) where T : Object{
        if (obj == null || destroyedObjects.Contains(obj)) {
            return;
        }

        destroyedObjects.Add(obj);

        if (levelsDown != 0) {
            SerializedObject sObj = new SerializedObject(obj);
            SerializedProperty it = sObj.GetIterator();

            while (it.Next(true)) {
                if (it.propertyType == SerializedPropertyType.ObjectReference) {
                    Object reference = it.objectReferenceValue;
                    if (reference != null && reference.GetType() == objType) {
                        destroyInternal(reference as T, objType, destroyedObjects, levelsDown - 1);
                    }
                }
            }
        }

        Object.DestroyImmediate(obj);
    }
}

}