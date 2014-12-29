using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class ReflectiveNode : Node {
    public string typeName;
    public string methodName;
    public string returnType;
    public string methodDisplayName;

    private List<MethodInfo> _methodInfos = null;

    private List<MethodInfo> methodInfos {
        get {
            if (_methodInfos == null) {
                loadMethodInfos();
            }
            return _methodInfos;
        }
    }

    private void loadMethodInfos() {
        System.Type type = DefaultValueNode.searchForType(typeName);
        name = type.Name;

        _methodInfos = new List<MethodInfo>();

        MethodInfo[] allInfos = type.GetMethods();
        foreach (MethodInfo info in allInfos) {
            if (info.Name == methodName) {
                if (info.ReturnType.FullName == returnType) {
                    _methodInfos.Add(info);
                }
            }
        }
    }

    private bool returnsVoid() {
        return returnType == "void";
    }

    public override void generateElements() {
        base.generateElements();

        loadMethodInfos();

        //if void, then it is executable 
        if (returnsVoid()) {
            ExecutionInputInfo executionIn = ScriptableObject.CreateInstance<ExecutionInputInfo>();
            executionIn.init(methodName, "void");
            elements.Add(executionIn);
        }

        Dictionary<string, System.Type> parameterIdToType = new Dictionary<string, System.Type>();
        foreach (MethodInfo methodInfo in methodInfos) {
            ParameterInfo[] parameters = methodInfo.GetParameters();
            foreach (ParameterInfo parameterInfo in parameters) {
                System.Type paramType;
                if (parameterIdToType.TryGetValue(parameterInfo.Name, out paramType)) {
                    if (paramType != parameterInfo.ParameterType) {
                        Debug.LogError("Two parameters with the same name but different types, cannot handle!");
                    }
                }

                parameterIdToType[parameterInfo.Name] = parameterInfo.ParameterType;
            }
        }

        foreach(var pair in parameterIdToType){
            InputWithDefaultInfo inputElement = ScriptableObject.CreateInstance<InputWithDefaultInfo>();
            inputElement.init(pair.Key, pair.Value.FullName);
            inputElement.initDefaultValue(this);
            elements.Add(inputElement);
        }

        if (returnsVoid()) {
            ExecutionOutInfo executionOut = ScriptableObject.CreateInstance<ExecutionOutInfo>();
            executionOut.init("out", "void");
            elements.Add(executionOut);
        } else {
            DataOutInfo returnValue = ScriptableObject.CreateInstance<DataOutInfo>();
            returnValue.init(methodDisplayName, returnType);
            elements.Add(returnValue);
        }

        getMethodStructs();
    }

    public override List<ExpressionMethodStruct> getExpressionStructs() {
        List<ExpressionMethodStruct> list = new List<ExpressionMethodStruct>();

        if (!returnsVoid()) {
            ScriptStructKey key = new ScriptStructKey(this, methodDisplayName);
            ExpressionMethodStruct s = new ExpressionMethodStruct(key, methodDisplayName, returnType);
            s.addCode(getExpressionCode());
            list.Add(s);
        }

        return list;
    }

    public override List<FieldStruct> getFieldStructs() {
        return new List<FieldStruct>();
    }

    public override List<GenericCodeStruct> getGenericCodeStructs() {
        return new List<GenericCodeStruct>();
    }

    public override List<MethodStruct> getMethodStructs() {
        List<MethodStruct> list = new List<MethodStruct>();

        //If return type is void, 2 lines of code
        //  call method with args
        //  ->out to exit
        if (returnsVoid()) {
            ScriptStructKey key = new ScriptStructKey(this, methodName);
            MethodStruct s = new MethodStruct(key, methodName);
            s.addCode(getExpressionCode() + ";");
            s.addCode("->out");
            list.Add(s);
        }

        return list;
    }

    private string getExpressionCode() {
        string expression = typeName + "." + methodName + "(";

        MethodInfo chosenMethod = null;
        ParameterInfo[] parameters;

        foreach (MethodInfo info in methodInfos) {
            parameters = info.GetParameters();

            bool canChooseMethod = true;

            foreach (ParameterInfo input in parameters) {
                InputWithDefaultInfo inputVar = getElement(input.Name) as InputWithDefaultInfo;
                if (inputVar == null) {
                    Debug.LogError("ID's did not match up!");
                    return null;
                }

                //A required input is not connected, abort
                if (!inputVar.isConnected()) {
                    canChooseMethod = false;
                    break;
                }
            }

            if (!canChooseMethod) {
                continue;
            }

            chosenMethod = info;
        }

        if (chosenMethod == null) {
            Debug.LogError("Could not find a suitable method for the given input config");
            return null;
        }

        parameters = chosenMethod.GetParameters();
        for (int i = 0; i < parameters.Length; i++) {
            ParameterInfo parameterInfo = parameters[i];

            expression += "<";
            expression += parameterInfo.ParameterType.FullName;
            expression += " ";
            expression += parameterInfo.Name;
            expression += ">";

            if (i != parameters.Length - 1) {
                expression += ",";
            }
        }
        expression += ")";
        Debug.Log(expression);
        return expression;
    }

}

}