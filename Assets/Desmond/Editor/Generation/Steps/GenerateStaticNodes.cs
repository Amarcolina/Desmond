using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class GenerateStaticNodes : GenerationStep {
    private Dictionary<string, Node> unanchoredStaticNodes = new Dictionary<string, Node>();
    private Dictionary<GameObject, Dictionary<string, Node>> anchoredStaticNodes = new Dictionary<GameObject, Dictionary<string, Node>>();

    private Dictionary<string, NodeFactory> nameToFactory = new Dictionary<string, NodeFactory>();

    public override void doStep() {
        foreach (NodeFactory factory in StringNodeFactory.getStaticStringNodeFactories()) {
            StringNodeFactory stringNodeFactory = factory as StringNodeFactory;
            nameToFactory[stringNodeFactory.nodeDescriptor.descriptorName] = stringNodeFactory;
        }
        
        foreach (Node node in nodes) {
            foreach (MethodStruct method in node.getMethodStructs()) {
                Node staticNode = getStaticNode(node.gameObjectInstance, method.staticReference);
                Element destinationElement = node.getElement(method.structKey.id);
                ConnectableElement originElement = staticNode.getElement("out") as ConnectableElement;
                Assert.that(originElement != null, "Element must exist and be of type ConnectableElement");

                ElementConnection proposedConnection = new ElementConnection(destinationElement, staticNode, node);
                Assert.that(originElement.tryConnect(proposedConnection), "static connection must not fail");
            }

            foreach (ExpressionMethodStruct expression in node.getExpressionStructs()) {
                Node staticNode = getStaticNode(node.gameObjectInstance, expression.staticReference);
                Element destinationElement = staticNode.getElement("out");
                ConnectableElement originElement = node.getElement(expression.structKey.id) as ConnectableElement;
                Assert.that(originElement != null, "Element must exist and be of type ConnectableElement");

                ElementConnection proposedConnection = new ElementConnection(destinationElement, staticNode, node);
                Assert.that(originElement.tryConnect(proposedConnection), "static connection must not fail");
            }
        }
    }

    private Node getStaticNode(GameObject anchor, string staticReference) {
        Dictionary<string, Node> nameToNode;
        if (anchor == null) {
            nameToNode = unanchoredStaticNodes;
        }else{
            if (!anchoredStaticNodes.TryGetValue(anchor, out nameToNode)) {
                nameToNode = new Dictionary<string, Node>();
                anchoredStaticNodes[anchor] = nameToNode;
            }
        }

        Node node;
        if (!nameToNode.TryGetValue(staticReference, out node)) {
            node = nameToFactory[staticReference].createNode();
            node.gameObjectInstance = null;
            nameToNode[staticReference] = node;
        }
        return node;
    }
}

}