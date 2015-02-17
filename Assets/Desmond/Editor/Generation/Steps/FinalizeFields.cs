using UnityEngine;
using System.Collections;

namespace Desmond { 

public class FinalizeFields : GenerationStep {

    public override void doStep() {
        LoadingBarUtil.beginChunk(scripts.Count, "", "", () => {
            foreach (ScriptStruct script in scripts.Values) {
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