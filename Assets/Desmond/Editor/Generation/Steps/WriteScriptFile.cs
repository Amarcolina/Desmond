using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class WriteScriptFile : GenerationStep {

    public override void doStep() {

        foreach (ScriptStruct script in scripts) {
            List<string> lines = new List<string>();

            foreach (string s in script.namespaceImports) {
                lines.Add("using " + s + ";");
            }

            lines.Add("");
            lines.Add("");

            lines.Add("public class " + script.ToString() + " : DesmondSceneScript {");

            foreach (FieldStruct field in script.fields.Values) {
                lines.AddRange(field.generateScriptLines());
            }

            lines.Add("");

            foreach (GenericMethodStruct method in script.methods.Values) {
                lines.AddRange(method.generateScriptLines());
                lines.Add("");
            }

            lines.Add("}");
        }

        string filePath = Application.dataPath + "/Desmond/Generated/SceneScripts/" + builder.scriptName + ".cs";
        System.IO.File.WriteAllLines(filePath, lines);
    }
}

}