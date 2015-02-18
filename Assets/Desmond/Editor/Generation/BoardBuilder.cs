using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class BoardBuilder {

    public static List<GenerationStep> steps = new List<GenerationStep>();

    public static List<PostCompilationJob> buildSceneBoards() {
        steps.Clear();

        steps.Add(new InitSceneBoards());
        steps.Add(new InitScriptStructs());
        steps.Add(new RecordNamespaceImports());
        steps.Add(new GenerateStaticNodes());
        steps.Add(new InitMethodStructs());
        steps.Add(new InitFields());
        steps.Add(new CountMethodReferences());
        steps.Add(new CountExpressionReferences());
        steps.Add(new CountFieldReferences());
        steps.Add(new ResolveScriptLocalNames());
        steps.Add(new PrepareMethodLocalNames());
        steps.Add(new FinalizeFields());
        steps.Add(new FinalizeExpressionMethods());
        steps.Add(new FinalizeMethods());
        steps.Add(new FinalizeMethodLocalNames());
        steps.Add(new WriteScriptFile());

        return doSteps();
    }

    private static List<PostCompilationJob> doSteps() {
        LoadingBarUtil.beginChunk(steps.Count, "Building Scripts: ", "", () => {
            GenerationStep previousStep = null;
            foreach (GenerationStep step in steps) {
                if (previousStep != null) {
                    step.init(previousStep);
                }
                LoadingBarUtil.beginChunk(1, step.ToString(), "", () => {
                    step.doStep();
                });
                previousStep = step;
            }
        });

        return null;
    }
}

}