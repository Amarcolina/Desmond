using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class WriteScriptFile : GenerationStep {

    public override void doStep() {
        LoadingBarUtil.beginChunk(4, "", "", () => {
            ScriptIndenter indenter = new ScriptIndenter();
            indenter.addCode("/*");

            LoadingBarUtil.recordProgress("Namespace imports");
            foreach (string s in script.namespaceImports) {
                indenter.addCode("using " + s + ";");
            }

            indenter.addCode("");
            indenter.addCode("");

            indenter.addCode("public class " + script.scriptName + " : DesmondSceneScript {");

            LoadingBarUtil.recordProgress("Fields");
            foreach (FieldStruct field in script.fields.Values.Where(f => f.shouldBeWritten())) {
                indenter.addCode(field.generateScriptLines());
            }

            indenter.addCode("");

            LoadingBarUtil.recordProgress("Methods");
            foreach (GenericMethodStruct method in script.methods.Values.Where(m => m.shouldBeWritten())) {
                MethodStruct m = method as MethodStruct;
                if (m != null && m.shouldBeInlined()) {
                    continue;
                }
                indenter.addCode(method.generateScriptLines());
            }

            indenter.addCode("}");

            indenter.addCode("*/");

            LoadingBarUtil.recordProgress("Writing to disk");
            string filePath = Application.dataPath + "/Desmond/Generated/SceneScripts/" + script.scriptName + ".cs";
            System.IO.File.WriteAllLines(filePath, indenter.toArray());
        });
        
    }
}

}