using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class InitMethodStructs {
    HashSet<MethodStruct> visitedMethods = new HashSet<MethodStruct>();


    public void doStep(ref List<Node> nodes, ref Dictionary<GameObject, ScriptStruct> scripts) {
        foreach (ScriptStruct script in scripts.Values) {
            foreach (CustomMethodStruct customMethod in script.customMethods) {
                addMethod(customMethod);
            }
        }
    }

    private void addMethod(GenericMethodStruct genericMethod, Dictionary<GameObject, ScriptStruct> scripts) {
        MethodStruct asMethodStruct = genericMethod as MethodStruct;
        if (asMethodStruct != null) {
            if (visitedMethods.Contains(asMethodStruct)) {
                asMethodStruct.references++;
                return;
            }
            visitedMethods.Add(asMethodStruct);
        }

        Node node = genericMethod.structKey.parentNode;

        foreach (string line in genericMethod.codeBlock) {
            string trimmedLine = line.Trim();
            if (trimmedLine.StartsWith("->")) {
                string outId = trimmedLine.Substring(2);

                ConnectableElement connectedElement;
                if (node.getConnectedElement(outId, out connectedElement)) {
                    Node connectedNode = connectedElement.parentNode;

                    ScriptElementKey connectedKey = new ScriptElementKey();
                    connectedKey.parentNode = connectedNode;
                    connectedKey.id = connectedElement.id;

                    ScriptStruct connectedScript = scripts[connectedNode.gameObjectInstance];

                }
            }
        }
    }
}

}
