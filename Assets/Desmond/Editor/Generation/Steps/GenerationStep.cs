using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public abstract class GenerationStep {
    public Dictionary<GameObject, ScriptStruct> scripts;
    public List<Node> nodes;

    public abstract void doStep();

    public delegate void GenericMethodDelegate(MethodStruct connectedMethod);
    public delegate List<string> GenericMethodReplaceDelegate(MethodStruct connectedMethod);

    public void forEveryMethodLink(GenericMethodStruct genericMethod, GenericMethodReplaceDelegate function) {
        Node node = genericMethod.structKey.parentNode;

        for (int i = genericMethod.codeBlock.Count - 1; i >= 0; i--) {
            string trimmedLine = genericMethod.codeBlock[i].Trim();
            if (trimmedLine.StartsWith("->")) {
                string outId = trimmedLine.Substring(2);

                List<string> totalReplacementLines = new List<string>();
                bool shouldReplace = false;

                List<ElementConnection> connections = node.getConnections(outId);
                foreach (ElementConnection connection in connections) {
                    Element connectedElement = connection.connectedElement;
                    Node connectedNode = connectedElement.parentNode;

                    ScriptElementKey connectedKey = new ScriptElementKey();
                    connectedKey.parentNode = connectedNode;
                    connectedKey.id = connectedElement.id;

                    ScriptStruct connectedScript = scripts[connectedNode.gameObjectInstance];
                    MethodStruct connectedMethod = connectedScript.methods[connectedKey] as MethodStruct;

                    if (connectedMethod == null) {
                        Debug.LogError("Error");
                    }

                    List<string> replacementLines = function(connectedMethod);
                    if (replacementLines != null) {
                        shouldReplace = true;
                        totalReplacementLines.AddRange(replacementLines); 
                    }
                }

                if (shouldReplace) {
                    genericMethod.codeBlock.RemoveAt(i);
                    genericMethod.codeBlock.InsertRange(i, totalReplacementLines);
                }
            }
        }
    }

    public void forEveryMethodLink(GenericMethodStruct genericMethod, GenericMethodDelegate function) {
        GenericMethodReplaceDelegate newFuncton = delegate(MethodStruct target) {
            function(target);
            return null;
        };
        forEveryMethodLink(genericMethod, newFuncton);
    }

    public delegate void ExpressionMethodDelegate(ExpressionMethodStruct connectedExpression);
    public delegate string ExpressionMethodReplaceDelegate(ExpressionMethodStruct connectedExpression);

    public void forEveryExpressionLink(GenericMethodStruct genericMethod, ExpressionMethodReplaceDelegate function) {
        Node node = genericMethod.structKey.parentNode;

        for (int i = 0; i < genericMethod.codeBlock.Count; i++) {
            string line = genericMethod.codeBlock[i];

            foreach (string[] match in StringHelper.getMatchingBraces(line, s => s != null)) {
                Element expressionElement = node.getElement(match[0]);

                if (expressionElement != null) {
                    List<ElementConnection> connections = node.getConnections(match[0]);
                    if (connections.Count != 1) {
                        Debug.LogError("Incorrect number of arguments for " + match[0] + " in node " + node);
                    }

                    Element connectedElement = connections[0].connectedElement;
                    Node connectedNode = connectedElement.parentNode;

                    ScriptElementKey connectedKey = new ScriptElementKey();
                    connectedKey.parentNode = connectedNode;
                    connectedKey.id = connectedElement.id;

                    ScriptStruct connectedScript = scripts[connectedNode.gameObjectInstance];

                    ExpressionMethodStruct expressionMethod = connectedScript.methods[connectedKey] as ExpressionMethodStruct;

                    if (expressionMethod == null) {
                        Debug.LogError("Error");
                    }

                    string replacementLine = function(expressionMethod);

                    if(replacementLine != null){
                        line = line.Replace("<" + match[0] + ">", replacementLine);
                    }
                }
            }
        }
    }

    public void forEveryExpressionLink(GenericMethodStruct genericMethod, ExpressionMethodDelegate function) {
        ExpressionMethodReplaceDelegate newFuncton = delegate(ExpressionMethodStruct target) {
            function(target);
            return null;
        };
        forEveryExpressionLink(genericMethod, newFuncton);
    }
}

}