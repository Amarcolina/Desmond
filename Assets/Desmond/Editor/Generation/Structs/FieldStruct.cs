using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class FieldStruct : ScriptElementStruct{
    public string type;
    public string name = "UNINITIALIZED_FIELD_NAME";
    public object defaultValue; //null if uninitializes, string if initialized with code, PropertyValue if initialized serially
    public int references;

    public bool isPublic = false;

    public FieldStruct(ScriptElementKey key, string type, object defaultValue = null) : base(key){
        this.type = type;
        this.defaultValue = defaultValue;
    }

    public override bool shouldBeWritten() {
        return references > 0 || isPublic;
    }

    public override List<string> generateScriptLines() {
        List<string> scriptLines = new List<string>();

        //if (defaultValue != null && defaultValue is PropertyValue) {
        //    scriptLines.Add("[SerializeField");
        //}

        string line;
        line = isPublic ? "public " : "private ";
        line += type + " ";
        line += name;

        if (defaultValue != null && defaultValue is string) {
            line += " = " + (string)defaultValue + ";";
        }else{
            line += ";";
        }

        scriptLines.Add(line);
        return scriptLines;
    }
}

}