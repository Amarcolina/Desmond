using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class MessageMethodStruct : GenericMethodStruct {
    public string messageName;

    public MessageMethodStruct(ScriptElementKey key, string messageName)
        : base(key) {
            this.messageName = messageName;
    }

    public override bool shouldBeWritten() {
        return true;
    }

    public override List<string> generateScriptLines() {
        List<string> lines = new List<string>();
        lines.Add("void " + messageName + "() {");
        lines.AddRange(codeBlock);
        lines.Add("}");
        lines.Add("");
        return lines;
    }
}

}