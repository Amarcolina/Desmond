using UnityEngine;
using System.Linq;
using System.Collections;

namespace Desmond {

public class CountFieldReferences : GenerationStep {

    public override void doStep() {
        LoadingBarUtil.beginChunk(scripts.Count, "", "", () => {
            foreach (ScriptStruct script in scripts.Values) {
                LoadingBarUtil.beginChunk(script.methods.Values.Count, "", "Counting field references : ", () => {
                    foreach (GenericMethodStruct method in script.methods.Values.Where(m => m.shouldBeWritten())) {
                        forEveryFieldLink(method, field => field.references++);
                        LoadingBarUtil.recordProgress(method.ToString());
                    }
                });
            }
        });
    }
}

}