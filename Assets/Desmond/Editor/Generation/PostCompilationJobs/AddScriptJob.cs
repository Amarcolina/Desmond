using UnityEngine;
using System.Collections;

namespace Desmond { 

[System.Serializable]
public class AddScriptJob : PostCompilationJob {
    public GameObject objectToAddScriptTo;
    public string scriptName;

    public override int getPriority() {
        return 50;
    }

    public void init(GameObject objectToAddScriptTo, string scriptName) {
        this.objectToAddScriptTo = objectToAddScriptTo;
        this.scriptName = scriptName;
    }

    public override void doJob() {
        objectToAddScriptTo.AddComponent(scriptName);
    }
}

}