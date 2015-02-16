using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class WriteScriptFile : GenerationStep {

    public override void doStep() {

        LoadingBarUtil.beginChunk(scripts.Count, "", "Writing Script : ", () => {
            foreach (ScriptStruct script in scripts.Values) {
                LoadingBarUtil.beginChunk(4, "", "", () => {
                    ScriptIndenter indenter = new ScriptIndenter();

                    LoadingBarUtil.recordProgress("Namespace imports");
                    foreach (string s in script.namespaceImports) {
                        indenter.addCode("using " + s + ";");
                    }

                    indenter.addCode("");
                    indenter.addCode("");

                    indenter.addCode("public class " + script.scriptName + " : DesmondSceneScript {");

                    LoadingBarUtil.recordProgress("Fields");
                    foreach (FieldStruct field in script.fields.Values) {
                        indenter.addCode(field.generateScriptLines());
                    }

                    indenter.addCode("");

                    LoadingBarUtil.recordProgress("Methods");
                    foreach (GenericMethodStruct method in script.methods.Values) {
                        indenter.addCode(method.generateScriptLines());
                        indenter.addCode("");
                    }

                    indenter.addCode("}");

                    LoadingBarUtil.recordProgress("Writing to disk");
                    string filePath = Application.dataPath + "/Desmond/Generated/SceneScripts/" + script.scriptName + ".cs";
                    System.IO.File.WriteAllLines(filePath, indenter.toArray());
                });
                
            }
        });
        
    }
}

}