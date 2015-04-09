using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 
    
public class InitMethodStructs : GenerationStep{
    public override void doStep() {
        LoadingBarUtil.beginChunk(nodes.Count, "", "Initializing Methods : ", () => {
            foreach (Node node in nodes) {
                List<CustomMethodStruct> customMethodStructs = node.getCustomMethodStructs();
                List<MethodStruct> methodStructs = node.getMethodStructs();
                List<ExpressionMethodStruct> expressions = node.getExpressionStructs();

                foreach (CustomMethodStruct customMethod in customMethodStructs) {
                    addMethod(customMethod);
                }

                foreach (MethodStruct method in methodStructs) {
                    addMethod(method);
                }

                foreach (ExpressionMethodStruct expression in expressions) {
                    addMethod(expression);
                }

                LoadingBarUtil.recordProgress(node.ToString());
            }
        });
    }

    private void addMethod(GenericMethodStruct genericMethod) {
        script.methods[genericMethod.structKey] = genericMethod;
    }
}

}
