using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class BoardBuilder {

    public static List<PostCompilationJob> buildBoards(List<DesmondBoard> boards) {
        List<GenerationStep> steps = new List<GenerationStep>();

        steps.Add(new InitScriptStructs());
        steps.Add(new RecordNamespaceImports());
        steps.Add(new GenerateStaticNodes());
        steps.Add(new InitFields());
        steps.Add(new InitMethodStructs());
        steps.Add(new ResolveScriptLocalNames());
        steps.Add(new FinalizeExpressionMethods());
        steps.Add(new WriteScriptFile());

        GenerationStep previousStep = null;
        foreach (GenerationStep step in steps) {
            if (previousStep == null) {
                step.boards = boards;
                step.nodes = new List<Node>();
                step.scripts = new Dictionary<GameObject, ScriptStruct>();
            } else {
                step.boards = previousStep.boards;
                step.nodes = previousStep.nodes;
                step.scripts = previousStep.scripts;
            }
            step.doStep();
            previousStep = step;
        }

        return null;
    }

    public static List<PostCompilationJob> buildSceneBoards() {
        DesmondSceneBase[] scriptBases = GameObject.FindObjectsOfType<DesmondSceneBase>();
        foreach (DesmondSceneBase scriptBase in scriptBases) {
            GameObject.DestroyImmediate(scriptBase);
        }

        List<DesmondBoard> boards = BoardHandler.getSceneBoards();
        GameObject sceneObject = BoardHandler.getSceneBoardHolder().gameObject;

        ScriptBuilder.clearStaticVariables();

        ScriptBuilder mainBuilder = new ScriptBuilder("DesmondSceneScript", sceneObject);
        ScriptBuilder.objectToBuilder[sceneObject] = mainBuilder;

        List<ScriptBuilder> allBuilders = new List<ScriptBuilder>();
        allBuilders.Add(mainBuilder);

        HashSet<Node> linkedNodes = new HashSet<Node>();
        List<Node> finalNodes = new List<Node>();
        foreach (DesmondBoard board in boards) {
            foreach (Node node in board.nodesInBoard) {
                foreach (Element element in node.elements) {
                    ConnectableElement ce = element as ConnectableElement;
                    if (ce != null) {
                        foreach (ElementConnection connection in ce.connections) {
                            if (!linkedNodes.Contains(connection.originNode)) {
                                finalNodes.Add(connection.originNode);
                                linkedNodes.Add(connection.originNode);
                            }
                            if (!linkedNodes.Contains(connection.destinationNode)) {
                                finalNodes.Add(connection.destinationNode);
                                linkedNodes.Add(connection.destinationNode);
                            }
                        }
                    }
                }
            }
        }

        foreach (Node node in finalNodes) {
            if (node.gameObjectInstance == null) {
                node.gameObjectInstance = sceneObject;
            }
            if (!ScriptBuilder.objectToBuilder.ContainsKey(node.gameObjectInstance)) {
                string scriptName = StringHelper.toClassName(node.gameObjectInstance.name + "DesmondScript");
                ScriptBuilder newBuilder = new ScriptBuilder(scriptName, node.gameObjectInstance);
                ScriptBuilder.objectToBuilder[node.gameObjectInstance] = newBuilder;
                allBuilders.Add(newBuilder);
            }
        }

        foreach (Node node in finalNodes) {
            ScriptBuilder builder = ScriptBuilder.objectToBuilder[node.gameObjectInstance];
            builder.addNode(node);
        }

        foreach (ScriptBuilder builder in allBuilders) {
            builder.buildStaticNodes();
        }

        foreach (ScriptBuilder builder in allBuilders) {
            builder.countAllReferences();
        }

        foreach (ScriptBuilder builder in allBuilders) {
            builder.generateUniqueNames();
        }

        foreach (ScriptBuilder builder in allBuilders) {
            builder.resolveAllExpressions();
        }

        foreach (ScriptBuilder builder in allBuilders) {
            builder.resolveAllMethods();
        }

        foreach (ScriptBuilder builder in allBuilders) {
            builder.resolveAllGenericCode();
        }

        foreach (ScriptBuilder builder in allBuilders) {
            ScriptWriter writer = new ScriptWriter(builder, "DesmondSceneBase");
            writer.writeToFile();
        }

        List<PostCompilationJob> jobs = new List<PostCompilationJob>();
        foreach (ScriptBuilder builder in allBuilders) {
            jobs.AddRange(builder.getPostCompilationJobs());
        }
        jobs.Sort();

        AssetDatabase.Refresh();

        return jobs;
    }

    public static void buildPrefabBoard(DesmondBoard board) {
        if (board.boardType != DesmondBoardType.PREFAB_BOARD) {
            Debug.Log("This method buildPrefabBoard can only be called using prefab boards");
            return;
        }

        ScriptBuilder.clearStaticVariables();

        ScriptBuilder mainBuilder = new ScriptBuilder(board.scriptName);

        HashSet<Node> linkedNodes = new HashSet<Node>();
        List<Node> finalNodes = new List<Node>();
        foreach (Node node in board.nodesInBoard) {
            foreach (Element element in node.elements) {
                ConnectableElement ce = element as ConnectableElement;
                if (ce != null) {
                    foreach (ElementConnection connection in ce.connections) {
                        if (!linkedNodes.Contains(connection.originNode)) {
                            finalNodes.Add(connection.originNode);
                            linkedNodes.Add(connection.originNode);
                        }
                        if (!linkedNodes.Contains(connection.destinationNode)) {
                            finalNodes.Add(connection.destinationNode);
                            linkedNodes.Add(connection.destinationNode);
                        }
                    }
                }
            }
        }

        foreach (Node node in finalNodes) {
            if (node.gameObjectInstance != null) {
                Debug.LogError("Only non-gameobject nodes should be in boards of type " + board.boardType);
            }
        }

        foreach (Node node in finalNodes) {
            mainBuilder.addNode(node);
        }

        mainBuilder.buildStaticNodes();
        mainBuilder.countAllReferences();
        mainBuilder.generateUniqueNames();
        mainBuilder.resolveAllExpressions();
        mainBuilder.resolveAllMethods();
        mainBuilder.resolveAllGenericCode();

        ScriptWriter writer = new ScriptWriter(mainBuilder, "DesmondPrefabBase");
        writer.writeToFile();

        AssetDatabase.Refresh();
    }
}

}