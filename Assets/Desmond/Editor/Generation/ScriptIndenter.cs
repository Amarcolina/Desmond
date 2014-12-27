using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class ScriptIndenter {
    private List<string> script = new List<string>();
    private string indent = "";

    public void addCode(string line) {
        line = line.Trim();
        if (line.StartsWith("}")) {
            indent = indent.Remove(0, 4);
        }
        script.Add(indent + line);
        if (line.EndsWith("{")) {
            indent += "    ";
        }
    }

    public void addCode(List<string> list) {
        foreach (string line in list) {
            addCode(line);
        }
    }

    public string[] toArray() {
        return script.ToArray();
    }
}

}