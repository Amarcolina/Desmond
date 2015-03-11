using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public class PreparedMethodLocalNameTable {
    public Dictionary<ScriptStruct, Dictionary<string, string>> uniqueIdToName = new Dictionary<ScriptStruct, Dictionary<string, string>>();
}

public class PrepareMethodLocalNames : GenerationStep {

    public override void doStep() {
        PreparedMethodLocalNameTable data = new PreparedMethodLocalNameTable();

        Dictionary<string, string> uniqueIdToName = new Dictionary<string, string>();
        int idCounter = 1;

        LoadingBarUtil.beginChunk(script.methods.Values.Count, "", "Preparing method local names : ", () => {
            foreach (GenericMethodStruct method in script.methods.Values) {
                Dictionary<string, int> nameToId = new Dictionary<string, int>();

                for (int i = 0; i < method.codeBlock.Count; i++) {
                    string line = method.codeBlock[i];
                    foreach (string[] match in StringHelper.getMatchingBraces(line, s => s == "methodLocalName", s => s != null)) {
                        string name = match[1];
                        if (nameToId.ContainsKey(name)) {
                            Debug.LogError("Mutiple definitions of the same name!");
                        } else {
                            nameToId[name] = idCounter;
                            uniqueIdToName[idCounter.ToString()] = name;
                            line = line.Replace("<" + match[0] + " " + match[1] + ">", "<methodLocalId " + idCounter + ">");
                            idCounter++;
                        }
                    }

                    foreach (string[] match in StringHelper.getMatchingBraces(line, s => nameToId.ContainsKey(s))) {
                        string name = match[0];
                        line = line.Replace("<" + name + ">", "<methodLocalId " + nameToId[name] + ">");
                    }

                    method.codeBlock[i] = line;
                }

                LoadingBarUtil.recordProgress(method.ToString());
            }
        });

        data.uniqueIdToName[script] = uniqueIdToName;
        addData(data);
    }

}

}