using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class WriteScriptFile : GenerationStep {

    public override void doStep() {
        foreach (ScriptStruct script in scripts.Values) {
            ScriptIndenter indenter = new ScriptIndenter();

            foreach (string s in script.namespaceImports) {
                indenter.addCode("using " + s + ";");
            }

            indenter.addCode("");
            indenter.addCode("");

            indenter.addCode("public class " + script.scriptName + " : DesmondSceneScript {");

            foreach (FieldStruct field in script.fields.Values) {
                indenter.addCode(field.generateScriptLines());
            }

            indenter.addCode("");

            foreach (GenericMethodStruct method in script.methods.Values) {
                indenter.addCode(method.generateScriptLines());
                indenter.addCode("");
            }

            indenter.addCode("}");

            string filePath = Application.dataPath + "/Desmond/Generated/SceneScripts/" + script.scriptName + ".cs";
            System.IO.File.WriteAllLines(filePath, indenter.toArray());
        }
    }
}

}