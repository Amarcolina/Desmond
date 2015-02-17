using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class FieldStruct : ScriptElementStruct{
    public string type;
    public string name;
    public object defaultValue;
    public int references;

    public bool isPublic = false;

    public FieldStruct(ScriptElementKey key, string type, string name, object defaultValue = null) : base(key){
        this.type = type;
        this.name = "fooooy";
        this.defaultValue = defaultValue;
    }

    public override bool shouldBeWritten() {
        return references > 0 || isPublic;
    }

    public override List<string> generateScriptLines() {
        List<string> scriptLines = new List<string>();
        string line;
        line  = "private ";
        line += type + " ";
        line += name;

        if (defaultValue != null && defaultValue.GetType() == typeof(string)) {
            line += " " + (string)defaultValue + ";";
        }else{
            line += ";";
        }

        scriptLines.Add(line);
        return scriptLines;
    }
}

}