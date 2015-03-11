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
    public List<Node> nodes = new List<Node>();

    public Dictionary<ScriptElementKey, GenericMethodStruct> methods = new Dictionary<ScriptElementKey, GenericMethodStruct>();
    public Dictionary<ScriptElementKey, FieldStruct> fields = new Dictionary<ScriptElementKey, FieldStruct>();
    public Dictionary<ScriptElementKey, string> scriptLocalNames = new Dictionary<ScriptElementKey, string>(); 

    public HashSet<string> namespaceImports = new HashSet<string>();

    public string scriptName;

    public IEnumerable<CustomMethodStruct> customMethods() {
        foreach (GenericMethodStruct genericMethod in methods.Values) {
            if (genericMethod is CustomMethodStruct) {
                yield return genericMethod as CustomMethodStruct;
            }
        }
    }

    public IEnumerable<MethodStruct> regularMethods() {
        foreach (GenericMethodStruct genericMethod in methods.Values) {
            if (genericMethod is MethodStruct) {
                yield return genericMethod as MethodStruct;
            }
        }
    }

    public IEnumerable<ExpressionMethodStruct> expressionMethods() {
        foreach (GenericMethodStruct genericMethod in methods.Values) {
            if (genericMethod is ExpressionMethodStruct) {
                yield return genericMethod as ExpressionMethodStruct;
            }
        }
    }
}


}