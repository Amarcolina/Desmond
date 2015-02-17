using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class ExpressionMethodStruct : MethodStruct {
    public string returnType;

    public ExpressionMethodStruct(ScriptElementKey key, string methodName, string returnType) : base(key, methodName) {
        this.returnType = returnType;
    }

    public override bool shouldBeInlined() {
        if (codeBlock.Count != 1) {
            return false;
        }

        return base.shouldBeInlined();
    }

    public override List<string> generateScriptLines() {
        List<string> scriptLines = new List<string>();

        string header = isPublic ? "public " : "private ";
        header += returnType + " ";
        header += methodName;
        header += "() {";

        scriptLines.Add(header);

        if (codeBlock.Count == 1) {
            scriptLines.Add("return " + codeBlock[0] + ";");
        }else{
            scriptLines.AddRange(codeBlock);
        }
        
        scriptLines.Add("}");

        return scriptLines;
    }
}

}