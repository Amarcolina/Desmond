using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class ScriptWriter {
    private string extendedClass = null;
    private List<string> classComments = new List<string>();
    private ScriptBuilder builder;

    public ScriptWriter(ScriptBuilder builder, string extendedClass) {
        this.builder = builder;
        this.extendedClass = extendedClass;
    }

    public void addClassComment(string comment) {
        classComments.Add(comment);
    }

    public void writeToFile(bool debug = false) {
        string[] lines = getLines(debug);
        if (debug) {
            Debug.Log("Writing " + lines.Length + " lines to " + builder.scriptName + ".cs");
        }
        string filePath = Application.dataPath + "/Desmond/Generated/" + builder.scriptName + ".cs";
        System.IO.File.WriteAllLines(filePath, lines);
    }

    public string[] getLines(bool debug) {
        ScriptIndenter scriptIndenter = new ScriptIndenter();

        scriptIndenter.addCode("using UnityEngine;");
        foreach (string namespaceImport in builder.namespaceImports) {
            scriptIndenter.addCode("using " + namespaceImport + ";");
        }

        scriptIndenter.addCode("");
        scriptIndenter.addCode("");
        scriptIndenter.addCode(classComments);
        if (extendedClass == null) {
            scriptIndenter.addCode("public class " + builder.scriptName +" {");
        } else {
            scriptIndenter.addCode("public class " + builder.scriptName + " : " + extendedClass + "{");
        }

        foreach (ScriptStruct s in builder.scriptFields) {
            scriptIndenter.addCode(s.getLines());
        }

        foreach (ScriptStruct s in builder.defaultFields) {
            if (s.shouldPrint()) {
                scriptIndenter.addCode(s.getLines());
            } else if (debug) {
                scriptIndenter.addCode("// " + s.getLines()[0] + "\t\t //REFERENCES:" + s.references);
            }
        }

        foreach (ScriptStruct s in builder.methodStructs) {
            if (s.shouldPrint()) {
                scriptIndenter.addCode("");
                scriptIndenter.addCode(s.getLines());
            } else if (debug) {
                scriptIndenter.addCode("");
                scriptIndenter.addCode("//References:" + s.references);
                scriptIndenter.addCode("/*");
                scriptIndenter.addCode(s.getLines());
                scriptIndenter.addCode("*/");
            }
        }

        foreach (ScriptStruct s in builder.codeStructs) {
            scriptIndenter.addCode("");
            scriptIndenter.addCode(s.getLines());
        }

        foreach (ScriptStruct s in builder.expressionStructs) {
            if (s.shouldPrint()) {
                scriptIndenter.addCode("");
                scriptIndenter.addCode(s.getLines());
            } else if (debug) {
                scriptIndenter.addCode("");
                scriptIndenter.addCode("//References:" + s.references);
                scriptIndenter.addCode("/*");
                scriptIndenter.addCode(s.getLines());
                scriptIndenter.addCode("*/");
            }
        }

        scriptIndenter.addCode("}");

        return scriptIndenter.toArray();
    }
}

}