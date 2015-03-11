using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public class GenerateStaticNodes : GenerationStep {
    private Dictionary<string, Node> staticNodes = new Dictionary<string, Node>();

    private Dictionary<string, NodeFactory> nameToFactory = new Dictionary<string, NodeFactory>();

    public override void doStep() {
        foreach (NodeFactory factory in StringNodeFactory.getStaticStringNodeFactories()) {
            StringNodeFactory stringNodeFactory = factory as StringNodeFactory;
            nameToFactory[stringNodeFactory.nodeDescriptor.descriptorName] = stringNodeFactory;
        }

        LoadingBarUtil.beginChunk(nodes.Count, "", "Generating Static Nodes : ", () => {
            foreach (Node node in nodes) {
                foreach (MethodStruct method in node.getMethodStructs()) {
                    if (method.staticReference != null) {
                        Node staticNode = getStaticNode(method.staticReference);
                        Element destinationElement = node.getElement(method.structKey.id);
                        ConnectableElement originElement = staticNode.getElement(method.structKey.id) as ConnectableElement;
                        Assert.that(originElement is ConnectableElement, "Static element " + method.structKey.id + " could not be found in node " + method.staticReference);
                        Assert.that(destinationElement is ExecutionInputInfo, "Destination must be of type execution input!");

                        ElementConnection proposedConnection = new ElementConnection(destinationElement, staticNode, node);
                        Assert.that(originElement.tryConnect(proposedConnection), "static connection must not fail");
                    }
                }
                
                foreach (ExpressionMethodStruct expression in node.getExpressionStructs()) {
                    if (expression.staticReference != null) {
                        Node staticNode = getStaticNode(expression.staticReference);
                        Element destinationElement = staticNode.getElement("out");
                        ConnectableElement originElement = node.getElement(expression.structKey.id) as ConnectableElement;
                        Assert.that(originElement != null, "Element must exist and be of type ConnectableElement");

                        ElementConnection proposedConnection = new ElementConnection(destinationElement, staticNode, node);
                        Assert.that(originElement.tryConnect(proposedConnection), "static connection must not fail");
                    }
                }

                LoadingBarUtil.recordProgress(node.ToString());
            }
        });

        nodes.AddRange(staticNodes.Values);
    }

    private Node getStaticNode(string staticReference) {
        Node node;
        if (!staticNodes.TryGetValue(staticReference, out node)) {
            node = nameToFactory[staticReference].createNode();
            staticNodes[staticReference] = node;
        }
        return node;
    }
}

}