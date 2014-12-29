using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public class ReflectiveNodeFactory : NodeFactory {
    private string typeName;
    private string methodName;
    private string returnType;
    private string methodDisplayName;

    public static List<NodeFactory> getReflectiveNodeFactories() {
        List<NodeFactory> list = new List<NodeFactory>();

        Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        foreach (var a in assemblies) {
            foreach (System.Type type in a.GetTypes()) {
                if (type.Namespace == "UnityEngine") {
                    MethodInfo[] infos = type.GetMethods(BindingFlags.Public | BindingFlags.Static);

                    Dictionary<string, HashSet<System.Type>> methodToReturnType = new Dictionary<string, HashSet<System.Type>>();
                    foreach (MethodInfo info in infos) {
                        HashSet<System.Type> returnTypeList;
                        if (!methodToReturnType.TryGetValue(info.Name, out returnTypeList)) {
                            returnTypeList = new HashSet<System.Type>();
                            methodToReturnType[info.Name] = returnTypeList;
                        }
                        returnTypeList.Add(info.ReturnType);
                    }
                    foreach (var pair in methodToReturnType) {
                        string methodName = pair.Key;
                        HashSet<System.Type> returnTypes = pair.Value;
                        foreach (System.Type returnType in returnTypes) {
                            //Only display return types if there are more than 1 to choose from
                            ReflectiveNodeFactory factory = new ReflectiveNodeFactory(type.FullName, methodName, returnType, returnTypes.Count > 1);
                            list.Add(factory);
                        }
                    }
                }
            }
        }

        return list;
    }

    public ReflectiveNodeFactory(string typeName, string methodName, System.Type returnType, bool displayReturnType) {
        this.typeName = typeName;
        this.methodName = methodName;
        this.returnType = returnType.FullName;

        methodDisplayName = methodName;
        if (methodDisplayName.StartsWith("get_")) {
            methodDisplayName = methodDisplayName.Replace("get_", "Get");
            methodDisplayName = StringHelper.capitalize(methodDisplayName, 3);
        }
        if (methodDisplayName.StartsWith("set_")) {
            methodDisplayName = methodDisplayName.Replace("set_", "Set");
            methodDisplayName = StringHelper.capitalize(methodDisplayName, 3);
        }

        if (displayReturnType) {
            methodDisplayName += " (" + returnType.Name + ")";
        }
    }

    public override Node createNode() {
        ReflectiveNode node = ScriptableObject.CreateInstance<ReflectiveNode>();
        node.typeName = typeName;
        node.methodName = methodName;
        node.returnType = returnType;
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
