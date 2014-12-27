using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public abstract class NodeFactory : IPathable {
    public abstract Node createNode();
    public abstract string getPath();

    public static List<NodeFactory> getAllFactories() {
        List<NodeFactory> list = new List<NodeFactory>();
        list.AddRange(StringNodeFactory.getStringNodeFactories());
        list.AddRange(ReflectiveNodeFactory.getReflectiveNodeFactories());
        return list;
    }
}

}