using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public class ScriptLocalGeneratedNames {
    public Dictionary<ScriptStruct, HashSet<string>> allScriptsGeneratedNames = new Dictionary<ScriptStruct, HashSet<string>>();
}

public class ResolveScriptLocalNames : GenerationStep {
    private HashSet<string> takenNames = new HashSet<string>();

    public override void doStep() {
        ScriptLocalGeneratedNames data = new ScriptLocalGeneratedNames();

        foreach (ScriptStruct script in scripts.Values) {
            Dictionary<ScriptElementKey, string> elementToFinal = new Dictionary<ScriptElementKey, string>();
            takenNames.Clear();

            foreach (GenericMethodStruct method in script.methods.Values) {
                //Generates names of named methods
                MethodStruct namedMethod = method as MethodStruct;
                if (namedMethod != null) {
                    namedMethod.methodName = generateUniqueName(namedMethod.structKey.id);
                }

                //Generate requestes scriptLocal names
                for (int i = 0; i < method.codeBlock.Count; i++){
                    string line = method.codeBlock[i];

                    foreach (string[] match in StringHelper.getMatchingBraces(line, s => s == "scriptLocalName", s => s != null)) {
                        string id = match[1];

                        ScriptElementKey key = new ScriptElementKey(method.structKey.parentNode, id);
                        if (elementToFinal.ContainsKey(key)) {
                            Debug.LogError("Mutiple definitions of the same key!");
                        }

                        string uniqueName = generateUniqueName(id);
                        elementToFinal[key] = uniqueName;
                        line.Replace("<" + match[0] + " " + match[1] + ">", uniqueName);
                    }

                    foreach (string[] match in StringHelper.getMatchingBraces(line, s => s != null)) {
                        string id = match[0];

                        ScriptElementKey key = new ScriptElementKey(method.structKey.parentNode, id);
                        if (!elementToFinal.ContainsKey(key)) {
                            continue; //fail silently cause it might be something else
                        }

                        line.Replace("<" + id + ">", elementToFinal[key]);
                    }

                    method.codeBlock[i] = line;
                }
            }

            foreach (FieldStruct field in script.fields.Values) {
                field.name = generateUniqueName(field.structKey.id);
            }

            data.allScriptsGeneratedNames[script] = takenNames;
        }

        addData(data);
    }

    private string generateUniqueName(string proposedName) {
        int nameIndex = 1;
        string uniqueName = proposedName;
        while (takenNames.Contains(uniqueName)) {
            nameIndex++;
            uniqueName = proposedName + nameIndex;
        }
        takenNames.Add(uniqueName);
        return uniqueName;
    }
}

}