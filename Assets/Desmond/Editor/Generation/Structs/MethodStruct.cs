using UnityEngine;
using System.Collections;
using System.Collections.Generic;   

namespace Desmond { 

public class MethodStruct : GenericMethodStruct {
    public InlineBehavior inlineBehavior;

    //Not part of original definition, but defined during script generation
    public bool isPublic = false;
    public string methodName = "undefined";

    public MethodStruct(ScriptElementKey key, string methodName) : base(key) {
        this.methodName = methodName;
    }

    public override bool shouldBeWritten() {
        return references > 0 || isPublic;
    }

    public virtual bool shouldBeInlined() {
        if(isPublic){
            return false;
        }

        switch (inlineBehavior) {
            case InlineBehavior.AUTO: {
                return references <= 1;
            }
            case InlineBehavior.FORCE_INLINE: {
                return true;
            }
            case InlineBehavior.PREVENT_INLINE: {
                return false;
            }
        }

        return false;
    }

    public override List<string> generateScriptLines() {
        List<string> scriptLines = new List<string>();

        string header = isPublic ? "public void " : "private void ";
        header += methodName;
        header += "() {";

        scriptLines.Add(header);
        scriptLines.AddRange(codeBlock);
        scriptLines.Add("}");
        scriptLines.Add("");

        return scriptLines;
    }
}

}