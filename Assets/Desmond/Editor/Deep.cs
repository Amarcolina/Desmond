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

        Object newObj = copyInternal(obj, objType, -1);
        oldToNew.Clear();

        return newObj;
    }

    public static Object copySurface(Object obj) {
        Assert.that(obj is IDeepObject);
        System.Type objType = obj.GetType();

        Object newObj = copyInternal(obj, objType, 1);
        oldToNew.Clear();
        return newObj;
    }

    private static Dictionary<Object, Object> oldToNew = new Dictionary<Object, Object>();
    private static Object copyInternal(Object obj, System.Type objType, int levelsDown) {
        if (oldToNew.ContainsKey(obj)) {
            return oldToNew[obj];
        }

        Object newObj = Object.Instantiate(obj);
        oldToNew[obj] = newObj;

        if (levelsDown != 0) {
            SerializedObject sObj = new SerializedObject(newObj);
            SerializedProperty it = sObj.GetIterator();

            while (it.Next(true)) {
                if (it.propertyType == SerializedPropertyType.ObjectReference) {
                    Object reference = it.objectReferenceValue;
                    if (reference != null && reference.GetType() == objType) {
                        it.objectReferenceValue = copyInternal(it.objectReferenceValue, objType, levelsDown - 1);
                    }
                }
            }

            sObj.ApplyModifiedProperties();
        }
        
        return newObj;
    }

    public static void destroy(Object obj) {
        Assert.that(obj is IDeepObject);
        System.Type objType = obj.GetType();

        destroyInternal(obj, objType, -1);
        destroyedObjects.Clear();
    }

    public static void destroySurface(Object obj) {
        Assert.that(obj is IDeepObject);
        System.Type objType = obj.GetType();

        destroyInternal(obj, objType, 1);
        destroyedObjects.Clear();
    }

    private static HashSet<Object> destroyedObjects = new HashSet<Object>();
    private static void destroyInternal(Object obj, System.Type objType, int levelsDown) {
        

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
                        destroyInternal(reference, objType, levelsDown - 1);
                    }
                }
            }
        }

        Object.DestroyImmediate(obj);
    }
}

}