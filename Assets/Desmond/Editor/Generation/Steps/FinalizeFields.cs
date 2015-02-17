using UnityEngine;
using System.Collections;

namespace Desmond { 

public class FinalizeFields : GenerationStep {

    public override void doStep() {
        LoadingBarUtil.beginChunk(scriptCount, "", "", () => {
            foreach (ScriptStruct script in scripts) {
                LoadingBarUtil.beginChunk(script.methods.Values.Count, "", "Finalizing Fields : ", () => {
                    foreach (GenericMethodStruct method in script.methods.Values) {
                        forEveryFieldLink(method, field => field.name);
                        LoadingBarUtil.recordProgress(method.ToString());
                    }
                });
            }
        });
    }
}

}