using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public class ReflectiveNodeFactory : NodeFactory {
    private string typeName;
    private string methodName;
    private string methodDisplayName;

    public static List<NodeFactory> getReflectiveNodeFactories() {
        List<NodeFactory> list = new List<NodeFactory>();

        Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        foreach (var a in assemblies) {
            foreach (System.Type type in a.GetTypes()) {
                if (type.Namespace == "UnityEngine") {
                    MethodInfo[] infos = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                    HashSet<string> methodNames = new HashSet<string>();
                    foreach (MethodInfo info in infos) {
                        methodNames.Add(info.Name);
                    }
                    foreach(string methodName in methodNames){
                        ReflectiveNodeFactory factory = new ReflectiveNodeFactory(type.FullName, methodName);
                        list.Add(factory);
                    }
                }
            }
        }

        return list;
    }

    public ReflectiveNodeFactory(string typeName, string methodName) {
        this.typeName = typeName;
        this.methodName = methodName;

        methodDisplayName = methodName;
        if (methodDisplayName.StartsWith("get_")) {
            methodDisplayName = methodDisplayName.Replace("get_", "get");
            methodDisplayName = StringHelper.capitalize(methodDisplayName, 3);
        }
        if (methodDisplayName.StartsWith("set_")) {
            methodDisplayName = methodDisplayName.Replace("set_", "set");
            methodDisplayName = StringHelper.capitalize(methodDisplayName, 3);
        }
    }

    public override Node createNode() {
        ReflectiveNode node = ScriptableObject.CreateInstance<ReflectiveNode>();
        node.typeName = typeName;
        node.methodName = methodName;
        node.methodDisplayName = methodDisplayName;
        node.generateElements();
        return node;
    }

    public override string getPath() {
        System.Type type = DefaultValueNode.searchForType(typeName);
        return "Reflective/" + type.Name + "/" + methodDisplayName;
    }
}

}
