using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public abstract class GenerationStep {
    public abstract void doStep(ref List<Node> nodes, ref Dictionary<GameObject, ScriptStruct> scripts);
}

}