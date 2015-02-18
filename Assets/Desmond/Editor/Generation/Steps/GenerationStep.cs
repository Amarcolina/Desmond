﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public abstract class GenerationStep {
    protected List<DesmondBoard> boards = new List<DesmondBoard>();

    private Dictionary<GameObject, ScriptStruct> anchoredScripts = new Dictionary<GameObject,ScriptStruct>();
    private ScriptStruct unanchoredScript = null;

    protected List<Node> nodes = new List<Node>();

    protected List<object> additionalData = new List<object>();

    public void init(GenerationStep previous) {
        boards = previous.boards;
        anchoredScripts = previous.anchoredScripts;
        unanchoredScript = previous.unanchoredScript;
        nodes = previous.nodes;
        additionalData = previous.additionalData;
    }

    protected int scriptCount {
        get {
            return anchoredScripts.Count + (unanchoredScript == null ? 0 : 1);
        }
    }

    protected IEnumerable<ScriptStruct> scripts {
        get {
            if (unanchoredScript != null) {
                yield return unanchoredScript;
            }
            foreach (ScriptStruct s in anchoredScripts.Values) {
                yield return s;
            }
        }
    }

    protected ScriptStruct getScript(GameObject anchor) {
        if (anchor == null) {
            return unanchoredScript;
        } else {
            return anchoredScripts[anchor];
        }
    }

    protected void setScript(GameObject anchor, ScriptStruct script) {
        if (anchor == null) {
            unanchoredScript = script;
        } else {
            anchoredScripts[anchor] = script;
        }
    }

    protected void addData(object o) {
        additionalData.Add(o);
    }

    protected T getData<T>(){
        foreach (object o in additionalData) {
            if(o.GetType() == typeof(T)){
                return (T)o;
            }
        }
        return default(T);
    }

    public abstract void doStep();

    public delegate void GenericMethodDelegate(MethodStruct connectedMethod);
    public delegate List<string> GenericMethodReplaceDelegate(MethodStruct connectedMethod);

    public void forEveryMethodLink(GenericMethodStruct genericMethod, GenericMethodReplaceDelegate function, bool shouldReplace = true) {
        Node node = genericMethod.structKey.parentNode;

        for (int i = genericMethod.codeBlock.Count - 1; i >= 0; i--) {
            string trimmedLine = genericMethod.codeBlock[i].Trim();
            if (trimmedLine.StartsWith("->")) {
                string outId = trimmedLine.Substring(2);

                List<string> totalReplacementLines = new List<string>();

                List<ElementConnection> connections = node.getConnections(outId);
                Assert.that(connections != null, "Connections " + outId + " could not be found");
                foreach (ElementConnection connection in connections) {
                    Element connectedElement = connection.destinationElement;
                    Node connectedNode = connectedElement.parentNode;

                    ScriptElementKey connectedKey = new ScriptElementKey();
                    connectedKey.parentNode = connectedNode;
                    connectedKey.id = connectedElement.id;

                    ScriptStruct connectedScript = getScript(connectedNode.gameObjectInstance);

                    GenericMethodStruct connectedGenericMethod;
                    Assert.that(connectedScript.methods.TryGetValue(connectedKey, out connectedGenericMethod), "Method " + connectedKey + " was not found on script " + connectedScript);
                    MethodStruct connectedMethod = connectedGenericMethod as MethodStruct;
                    Assert.that(connectedMethod != null, "Method must be of type MethodStruct");

                    List<string> replacementLines = function(connectedMethod);
                    if (replacementLines != null) {
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
        forEveryMethodLink(genericMethod, newFuncton, false);
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

                    Element connectedElement = connections[0].destinationElement;
                    Node connectedNode = connectedElement.parentNode;

                    ScriptElementKey connectedKey = new ScriptElementKey();
                    connectedKey.parentNode = connectedNode;
                    connectedKey.id = connectedElement.id;

                    ScriptStruct connectedScript = getScript(connectedNode.gameObjectInstance);
                    Assert.that(connectedScript != null, "Connected script cannot be null");

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

            genericMethod.codeBlock[i] = line;
        }
    }

    public void forEveryExpressionLink(GenericMethodStruct genericMethod, ExpressionMethodDelegate function) {
        ExpressionMethodReplaceDelegate newFuncton = delegate(ExpressionMethodStruct target) {
            function(target);
            return null;
        };
        forEveryExpressionLink(genericMethod, newFuncton);
    }

    public delegate void FieldStructDelegate(FieldStruct referencedField);
    public delegate string FieldStructReplaceDelegate(FieldStruct referencedField);

    public void forEveryFieldLink(GenericMethodStruct genericMethod, FieldStructReplaceDelegate function) {
        Node node = genericMethod.structKey.parentNode;

        for (int i = 0; i < genericMethod.codeBlock.Count; i++) {
            string line = genericMethod.codeBlock[i];

            foreach (string[] match in StringHelper.getMatchingBraces(line, s => s != null)) {
                ScriptStruct script = getScript(node.gameObjectInstance);
                ScriptElementKey key = new ScriptElementKey(node, match[0]);

                FieldStruct field;
                
                if(script.fields.TryGetValue(key, out field)){
                    string replacementLine = function(field);

                    if (replacementLine != null) {
                        line = line.Replace("<" + match[0] + ">", replacementLine);
                    }
                }
            }

            genericMethod.codeBlock[i] = line;
        }
    }

    public void forEveryFieldLink(GenericMethodStruct genericMethod, FieldStructDelegate function) {
        FieldStructReplaceDelegate newFuncton = delegate(FieldStruct target) {
            function(target);
            return null;
        };
        forEveryFieldLink(genericMethod, newFuncton);
    }
}

}