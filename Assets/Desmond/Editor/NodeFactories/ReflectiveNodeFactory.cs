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
    private bool isStatic;

    private struct MethodStruct {
        public string methodName;
        public bool isStatic;

        public MethodStruct(string methodName, bool isStatic) {
            this.methodName = methodName;
            this.isStatic = isStatic;
        }
    }

    public static List<NodeFactory> getReflectiveNodeFactories() {
        List<NodeFactory> list = new List<NodeFactory>();

        Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        foreach (var a in assemblies) {
            foreach (System.Type type in a.GetTypes()) {
                if (type.Namespace == "UnityEngine") {
                    MethodInfo[] infos = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                    Dictionary<MethodStruct, HashSet<System.Type>> methodToReturnType = new Dictionary<MethodStruct, HashSet<System.Type>>();
                    foreach (MethodInfo info in infos) {
                        MethodStruct methodStruct = new MethodStruct(info.Name, info.IsStatic);
                        HashSet<System.Type> returnTypeList;
                        if (!methodToReturnType.TryGetValue(methodStruct, out returnTypeList)) {
                            returnTypeList = new HashSet<System.Type>();
                            methodToReturnType[methodStruct] = returnTypeList;
                        }
                        returnTypeList.Add(info.ReturnType);
                    }
                    foreach (var pair in methodToReturnType) {
                        MethodStruct methodStruct = pair.Key;
                        string methodName = methodStruct.methodName;
                        bool isStatic = methodStruct.isStatic;
                        HashSet<System.Type> returnTypes = pair.Value;
                        foreach (System.Type returnType in returnTypes) {
                            //Only display return types if there are more than 1 to choose from
                            ReflectiveNodeFactory factory = new ReflectiveNodeFactory(type.FullName, methodName, returnType, isStatic, returnTypes.Count > 1);
                            list.Add(factory);
                        }
                    }
                }
            }
        }

        return list;
    }

    public ReflectiveNodeFactory(string typeName, string methodName, System.Type returnType, bool isStatic, bool displayReturnType) {
        this.typeName = typeName;
        this.methodName = methodName;
        this.returnType = returnType.FullName;
        this.isStatic = isStatic;

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
        node.isStatic = isStatic;
        node.generateElements();
        return node;
    }

    public override string getPath() {
        System.Type type = DefaultValueNode.searchForType(typeName);
        if (isStatic) {
            return "Reflective/" + type.Name + "/Static/" + methodDisplayName;
        } else {
            return "Reflective/" + type.Name + "/Instance/" + methodDisplayName;
        }
        
    }
}

}
