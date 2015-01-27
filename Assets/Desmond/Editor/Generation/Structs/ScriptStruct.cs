using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond{

public enum InlineBehavior {
    AUTO,
    FORCE_INLINE,
    PREVENT_INLINE
}

/* Represents a chunk of data that can be placed into a script
 * It belongs to a node, and belongs on a specific game object
 */
public class ScriptStruct{
    public GameObject parentObject;

    public List<Node> nodes = new List<Node>();

    public ScriptElementSet<GenericMethodStruct> methods = new ScriptElementSet<GenericMethodStruct>();

    public ScriptElementSet<FieldStruct> fields = new ScriptElementSet<FieldStruct>();

    public HashSet<string> namespaceImports = new HashSet<string>();

    public IEnumerable<CustomMethodStruct> customMethods() {
        foreach (GenericMethodStruct genericMethod in methods) {
            if (genericMethod is CustomMethodStruct) {
                yield return genericMethod as CustomMethodStruct;
            }
        }
    }

    public IEnumerable<MethodStruct> customMethods() {
        foreach (GenericMethodStruct genericMethod in methods) {
            if (genericMethod is MethodStruct) {
                yield return genericMethod as MethodStruct;
            }
        }
    }

    public IEnumerable<ExpressionMethodStruct> customMethods() {
        foreach (GenericMethodStruct genericMethod in methods) {
            if (genericMethod is ExpressionMethodStruct) {
                yield return genericMethod as ExpressionMethodStruct;
            }
        }
    }
}


}