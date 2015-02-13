using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class FieldStruct : ScriptElementStruct{
    public string type;
    public string name;
    public object defaultValue;
    public int references;

    public override List<string> generateScriptLines() {
        List<string> scriptLines = new List<string>();
        if (references == 0) {
            return scriptLines;
        }

        string line;
        line  = "private ";
        line += type + " ";
        line += name;

        if (defaultValue != null && defaultValue.GetType() == typeof(string)) {
            line = " " + (string)defaultValue + ";";
        }else{
            line += ";";
        }

        scriptLines.Add(line);
        return scriptLines;
    }
}

}