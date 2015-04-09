using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class FinalizeMethodLocalNames : GenerationStep {

    public override void doStep() {
        PreparedMethodLocalNameTable table = getData<PreparedMethodLocalNameTable>();

        LoadingBarUtil.beginChunk(script.methods.Values.Count, "", "Preparing method local names : ", () => {
            foreach (GenericMethodStruct method in script.methods.Values) {
                Dictionary<string, string> uniqueIdToFinal    = new Dictionary<string, string>();
                Dictionary<string, string> uniqueIdToProposed = table.uniqueIdToName[script];

                for (int i = 0; i < method.codeBlock.Count; i++) {
                    string line = method.codeBlock[i];

                    foreach (string[] match in StringHelper.getMatchingBraces(line, s => s == "methodLocalId", s => s != null)) {
                        string finalName;
                        if(!uniqueIdToFinal.TryGetValue(match[1], out finalName)){
                            string proposed;
                            Assert.that(uniqueIdToProposed.TryGetValue(match[1], out proposed));

                            int index = 1;
                            finalName = proposed;
                            while (uniqueIdToFinal.ContainsValue(finalName)) {
                                finalName = proposed + index;
                                index++;
                            }

                            uniqueIdToFinal[match[1]] = finalName;
                        }

                        line = line.Replace("<" + match[0] + " " + match[1] + ">", finalName);
                    }

                    method.codeBlock[i] = line;
                }
            }
        });
    }
}

}