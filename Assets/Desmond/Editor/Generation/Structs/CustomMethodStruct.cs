﻿using UnityEngine;
using System.Collections;

namespace Desmond { 

public class CustomMethodStruct : GenericMethodStruct {
    public CustomMethodStruct(ScriptElementKey key) : base(key) { }

    public override bool shouldBeWritten() {
        return true;
    }
}

}