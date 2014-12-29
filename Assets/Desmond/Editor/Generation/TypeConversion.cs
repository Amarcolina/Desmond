﻿using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public class TypeConversion {
    public static Dictionary<string, List<string>> typeToTypes = new Dictionary<string,List<string>>();
    public static Dictionary<TypeToType, string> setOfConversions = new Dictionary<TypeToType, string>();

    public struct TypeToType {
        public string type;
        public string fromType;

        public TypeToType(string t, string f) {
            type = t;
            fromType = f;
        }
    }

    public static void loadTypeConversion(string file) {
        string[] lines = System.IO.File.ReadAllLines(Application.dataPath + file);

        string fromType = null;
        string toType = null;
        
        foreach (string l in lines) {
            string[] st = l.Trim().Split();
            if (st[0] == "") {
                continue;
            }

            if (st.Length == 2) {
                fromType = st[0];
                toType = st[1];
            } else if (st.Length == 1){
                addPair(fromType, toType, st[0]);
            }
        }
    }

    public static void autoGenerateConversions() {
        int autoGeneratedConversions = 0;

        Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        foreach (var a in assemblies) {
            foreach (System.Type type in a.GetTypes()) {
                if (type.Namespace == "UnityEngine") {
                    MethodInfo[] infos = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                    foreach (MethodInfo info in infos) {
                        if (info.Name == "op_Implicit") {
                            ParameterInfo[] parameters = info.GetParameters();
                            if (parameters.Length != 1) {
                                continue;
                            }
                            System.Type toType = info.ReturnType;
                            System.Type fromType = parameters[0].ParameterType;

                            addPair(fromType.FullName, toType.FullName, "<in>");
                            autoGeneratedConversions++;
                        }
                    }
                }
            }
        }
    }

    public static void addBaseType(string typeName) {
        //Types can always be converted to their base types with no effort
        System.Type type = DefaultValueNode.searchForType(typeName);
        if (type == null) {
            Debug.LogError("Could not find type [" + typeName + "]");
            return;
        }
        System.Type baseType = type.BaseType;
        if (baseType != null) {
            addPair(typeName, baseType.FullName, "<in>");
        }
    }

    public static void addPair(string typeFrom, string typeTo, string code) {
        TypeToType typeToType = new TypeToType(typeFrom, typeTo);
        if (setOfConversions.ContainsKey(typeToType)) {
            return;
        }

        setOfConversions[typeToType] = code;

        addBaseType(typeFrom);
        addBaseType(typeTo);

        List<string> list;
        if (!typeToTypes.TryGetValue(typeFrom, out list)) {
            list = new List<string>();
            typeToTypes[typeFrom] = list;
        }
        list.Add(typeTo);
    }

    public static bool canConvertTo(string from, string to) {
        string dummy;
        return tryCastExpression(from, "-", to, out dummy);
    }

    public static bool tryCastExpression(string inputType, string inputExpression, string outputType, out string outputExpression) {
        string pattern;

        if (inputType == outputType) {
            outputExpression = inputExpression;
            return true;
        }

        if (outputType == "object" && inputType != "void") {
            outputExpression = inputExpression;
            return true;
        }

        if (inputType == "object" && outputType != "void") {
            outputExpression = "((" + outputType + ")" + inputExpression + ")";
            return true;
        }

        if (outputType == "System.String") {
            if(inputType == "System.Int32" || inputType == "System.Single"){
                outputExpression = "(" + inputExpression + " + \"\")";
            }else{
                outputExpression = "(" + inputExpression + ".ToString())";
            }
            return true;
        }

        if(!setOfConversions.TryGetValue(new TypeToType(inputType, outputType), out pattern)){
            searchForNewPair(inputType, outputType);
            setOfConversions.TryGetValue(new TypeToType(inputType, outputType), out pattern);
        }

        if (pattern == "") {
            outputExpression = null;
            return false;
        }

        outputExpression = pattern.Replace("<in>", inputExpression);
        return true;
    }

    public static bool searchForNewPair(string inputType, string outputType) {
        Dictionary<string, string> typeBacktrack = new Dictionary<string, string>();
        HashSet<string> visitedTypes = new HashSet<string>();
        Queue<string> typesToCheck = new Queue<string>();
        typesToCheck.Enqueue(inputType);

        while (typesToCheck.Count != 0) {
            string currentType = typesToCheck.Dequeue();

            if (!typeToTypes.ContainsKey(currentType)) {
                continue;
            }

            List<string> connectedTypes = typeToTypes[currentType];
            foreach (string s in connectedTypes) {
                if (s == outputType) {
                    typeBacktrack[s] = currentType;

                    string generatedExpression = "<in>";
                    string type = s;
                    while (type != inputType) {
                        string linkedType = typeBacktrack[type];
                        generatedExpression = generatedExpression.Replace("<in>", setOfConversions[new TypeToType(linkedType, type)]);
                        type = linkedType;
                    }

                    addPair(inputType, outputType, generatedExpression);

                    return true;
                }

                if (!visitedTypes.Contains(s)) {
                    visitedTypes.Add(s);
                    typesToCheck.Enqueue(s);
                    typeBacktrack[s] = currentType;
                }
            }
        }

        addPair(inputType, outputType, "");
        return false;
    }

    private static string expressionDo(string s, string pattern){
        return pattern.Replace("<in>", s);
    }
}

}
