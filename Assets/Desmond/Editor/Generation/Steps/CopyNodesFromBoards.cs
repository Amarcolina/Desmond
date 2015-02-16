using UnityEngine;
using System.Collections;

namespace Desmond { 

public class CopyNodesFromBoards : GenerationStep {

    public override void doStep() {
        foreach (DesmondBoard board in boards) {
            foreach (Node node in board.nodesInBoard) {
                nodes.Add(Object.Instantiate(node) as Node);
            }
        }
    }
}

}
