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
    public bool isStatic;
    public bool isGetSet;

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
        return returnType == "System.Void";
    }

    public override void generateElements() {
        base.generateElements();

        loadMethodInfos(); 

        //if void, then it is executable 
        if (returnsVoid()) {
            ExecutionInputInfo executionIn = ScriptableObject.CreateInstance<ExecutionInputInfo>();
            executionIn.init(methodDisplayName, "void", this);
            elements.Add(executionIn);
        }

        if (!isStatic) {
            InputWithDefaultInfo inputElement = ScriptableObject.CreateInstance<InputWithDefaultInfo>();
            inputElement.init("instance", typeName, this);
            elements.Add(inputElement);
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
            inputElement.init(pair.Key, pair.Value.FullName, this);
            elements.Add(inputElement);
        }

        if (returnsVoid()) {
            ExecutionOutInfo executionOut = ScriptableObject.CreateInstance<ExecutionOutInfo>();
            executionOut.init("out", "void", this);
            elements.Add(executionOut);
        } else {
            DataOutInfo returnValue = ScriptableObject.CreateInstance<DataOutInfo>();
            returnValue.init(methodDisplayName, returnType, this);
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
        MethodInfo chosenMethod = chooseMethodGivenInputs();

        if (isGetSet) {
            return getGetSetExpression(chosenMethod);
        } else {
            string op = getImplicitOperator();
            if (op != null && chosenMethod.GetParameters().Length == 2) {
                return getOperatorExpression(op, chosenMethod);
            }else{
                return getMethodExpression(chosenMethod);
            }
        }
    }

    public string getMethodExpression(MethodInfo chosenMethod) {
        string expression = "";

        if (isStatic) {
            expression = typeName + ".";
        } else {
            expression = "<" + typeName + " instance>.";
        }

        expression += methodName + "(" + getCommaDelimitedArguments(chosenMethod) + ")";

        return expression;
    }

    public string getGetSetExpression(MethodInfo chosenMethod) {
        string expression = "";

        if (isStatic) {
            expression = typeName + ".";
        } else {
            expression = "<" + typeName + " instance>.";
        }

        //If it's a getter
        ParameterInfo[] parameters = chosenMethod.GetParameters();
        if (parameters.Length == 1) {
            expression += " = ";
            expression += getParameterExpression(chosenMethod.GetParameters()[0]);
        } else if (parameters.Length != 0) {
            Debug.LogError("Getter or Setter method should either have 1 or 0 arguments");
        }

        return expression;
    }

    public string getOperatorExpression(string op, MethodInfo chosenMethod) {
        if (!isStatic) {
            Debug.LogError("Only static operators should be allowed");
        }

        string expression = "";

        ParameterInfo[] parameters = chosenMethod.GetParameters();

        expression += getParameterExpression(parameters[0]);
        expression += " " + op + " ";
        expression += getParameterExpression(parameters[1]);

        return expression;
    }

    public MethodInfo chooseMethodGivenInputs() {
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

        return chosenMethod;
    }

    public string getCommaDelimitedArguments(MethodInfo method) {
        string expression = "";

        ParameterInfo[] parameters = method.GetParameters();
        for (int i = 0; i < parameters.Length; i++) {
            ParameterInfo parameterInfo = parameters[i];

            expression += getParameterExpression(parameterInfo);

            if (i != parameters.Length - 1) {
                expression += ",";
            }
        }

        return expression;
    }

    public string getParameterExpression(ParameterInfo parameter) {
        return "<" + parameter.ParameterType.FullName + " " + parameter.Name + ">";
    }

    public string getImplicitOperator() {
        if (!methodName.StartsWith("op_")) {
            return null;
        }
        string opName = methodName.Substring(3);
        if (opName.StartsWith("Equality")) {
            return "==";
        }
        if (opName.StartsWith("Inequality")) {
            return "!=";
        }
        if (opName.StartsWith("Addition")) {
            return "+";
        }
        if (opName.StartsWith("Subtraction")) {
            return "-";
        }
        if (opName.StartsWith("Multiplication")) {
            return "*";
        }
        if (opName.StartsWith("Division")) {
            return "/";
        }

        return null;
    }

}

}