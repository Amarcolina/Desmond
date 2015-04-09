using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public class StringNodeFactory : NodeFactory {
    public NodeDescriptor nodeDescriptor;

    public static List<NodeFactory> getStringNodeFactories() {
        List<NodeFactory> list = new List<NodeFactory>();

        foreach (NodeDescriptor descriptor in NodeDescriptor.descriptors.Values) {
            StringNodeFactory nodeFactory = new StringNodeFactory(descriptor);
            list.Add(nodeFactory);
        }

        return list;
    }

    public static List<NodeFactory> getStaticStringNodeFactories() {
        List<NodeFactory> list = new List<NodeFactory>();

        foreach (NodeDescriptor descriptor in NodeDescriptor.staticDescriptors.Values) {
            StringNodeFactory nodeFactory = new StringNodeFactory(descriptor);
            list.Add(nodeFactory);
        }

        return list;
    }

    public StringNodeFactory(NodeDescriptor nodeDescriptor) {
        this.nodeDescriptor = nodeDescriptor;
    }

    public override Node createNode() {
        StringNode node = ScriptableObject.CreateInstance<StringNode>();
        node.descriptor = nodeDescriptor;
        node.name = nodeDescriptor.descriptorName;
        node.generateElements();
        return node;
    }

    public override string getPath() {
        return nodeDescriptor.descriptorName;
    }
}

}
