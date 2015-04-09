using UnityEngine;
using System.Collections;

namespace Desmond { 

public class CleanupGeneration : GenerationStep {

    public override void doStep() {
        foreach (DesmondBoard board in boards) {
            Deep.collectOwnedObjects(board);
        }
        foreach (Node node in nodes) {
            Deep.collectOwnedObjects(node);
        }
        Deep.destroy();
    }
}

}