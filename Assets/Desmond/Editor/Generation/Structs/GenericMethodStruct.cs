﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public class GenericMethodStruct : ScriptElementStruct{
    public List<string> codeBlock;
    public int references = 0;

    public GenericMethodStruct(ScriptElementKey key) : base(key) { }

    public override List<string> generateScriptLines() {
        return codeBlock;
    }

    public void addCode(string line) {
        codeBlock.Add(line);
    }

    public void addCode(List<string> lines) {
        foreach (string line in lines) {
            addCode(line);
        }
    }
}

}