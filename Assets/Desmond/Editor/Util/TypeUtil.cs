using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class TypeUtil {
    private static Dictionary<string, System.Type> _typeMap = new Dictionary<string, System.Type>();

    public static System.Type searchForType(string fullName) {
        System.Type type;
        if (_typeMap.TryGetValue(fullName, out type)) {
            return type;
        }

        System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        foreach (var A in assemblies) {
            type = System.Type.GetType(fullName + "," + A.FullName);
            if (type != null) {
                _typeMap[fullName] = type;
                return type;
            }
        }

        return null;
    }
}

}