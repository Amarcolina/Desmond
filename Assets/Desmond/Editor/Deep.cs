using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public interface IDeepObject {
}

public static class Deep {

    public static Object copy(Object obj) {
        Assert.that(obj is IDeepObject);
        System.Type objType = obj.GetType();

        Object newObj = copyInternal(obj, objType);
        oldToNew.Clear();

        return newObj;
    }

    private static Dictionary<Object, Object> oldToNew = new Dictionary<Object, Object>();
    private static Object copyInternal(Object obj, System.Type objType) {
        if (oldToNew.ContainsKey(obj)) {
            return oldToNew[obj];
        }

        Object newObj = Object.Instantiate(obj);
        oldToNew[obj] = newObj;
        SerializedObject sObj = new SerializedObject(newObj);
        SerializedProperty it = sObj.GetIterator();

        while (it.Next(true)) {
            if (it.propertyType == SerializedPropertyType.ObjectReference) {
                Object reference = it.objectReferenceValue;
                if (reference != null && reference.GetType() == objType) {
                    it.objectReferenceValue = copyInternal(it.objectReferenceValue, objType);
                }
            }
        }

        sObj.ApplyModifiedProperties();
        return newObj;
    }

    public static void destroy(Object obj) {
        Assert.that(obj is IDeepObject);
        System.Type objType = obj.GetType();

        destroyInternal(obj, objType);
        destroyedObjects.Clear();
    }

    private static HashSet<Object> destroyedObjects = new HashSet<Object>();
    private static void destroyInternal(Object obj, System.Type objType) {
        if (obj == null || destroyedObjects.Contains(obj)) {
            return;
        }

        destroyedObjects.Add(obj);
        SerializedObject sObj = new SerializedObject(obj);
        SerializedProperty it = sObj.GetIterator();

        while (it.Next(true)) {
            if (it.propertyType == SerializedPropertyType.ObjectReference) {
                Object reference = it.objectReferenceValue;
                if (reference != null && reference.GetType() == objType) {
                    destroyInternal(reference, objType);
                }
            }
        }

        Object.DestroyImmediate(obj);
    }
}

}